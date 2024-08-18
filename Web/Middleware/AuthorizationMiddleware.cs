using API.Dtos;
using System.IO;
using System.Net.Http.Headers;

namespace Web.Middleware
{
    public class AuthorizationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<AuthorizationMiddleware> _logger;

        public AuthorizationMiddleware(RequestDelegate next, IHttpClientFactory clientFactory, IConfiguration configuration, ILogger<AuthorizationMiddleware> logger)
        {
            _next = next;
            _clientFactory = clientFactory;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var token = context.Session.GetString("Token");
            var path = context.Request.Path.Value;

            if (path.StartsWith("/Auth/Login", StringComparison.OrdinalIgnoreCase) || path.StartsWith("/public", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            if (!string.IsNullOrEmpty(token))
            {
                var userProfile = await GetUserProfileAsync(token);
                if (userProfile != null)
                {
                    var hasAccess = await CheckAccessAsync(userProfile.RoleId, path);

                    if (!hasAccess)
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return;
                    }

                    context.Items["UserProfile"] = userProfile;
                }
                else
                {
                    //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    context.Response.Redirect("/Auth/Login");
                    return;
                }
            }
            else
            {
                //context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                context.Response.Redirect("/Auth/Login");
                return;
            }

            await _next(context);
        }

        private async Task<UserDTO> GetUserProfileAsync(string token)
        {
            var apiUrl = _configuration["ApiUrl"];
            using var client = _clientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

            var response = await client.GetAsync($"{apiUrl}/users/profile");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<UserDTO>();
            }

            _logger.LogError($"Failed to fetch user profile. Status code: {response.StatusCode}");
            return null;
        }

        private async Task<bool> CheckAccessAsync(int? roleId, string path)
        {
            var apiUrl = _configuration["ApiUrl"];
            using var client = _clientFactory.CreateClient();

            var response = await client.GetAsync($"{apiUrl}/rules/check-access?roleId={roleId}&path={path}");
            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<bool>();
            }

            _logger.LogError($"Failed to check access. Status code: {response.StatusCode}");
            return false;
        }
    }
}
