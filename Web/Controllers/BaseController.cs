using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using System.Text.Json;
using API.Dtos;
using API.Ultils;

namespace Web.Controllers
{
    public class BaseController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<BaseController> _logger;

        public BaseController(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<BaseController> logger)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        protected async Task<UserDTO> GetUserProfileAsync(string token)
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];

                using var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync($"{apiUrl}/users/profile");

                if (response.IsSuccessStatusCode)
                {
                    //var jsonString = await response.Content.ReadAsStringAsync();
                    //var userProfile = JsonSerializer.Deserialize<UserDTO>(jsonString);
                    var userProfile = await response.Content.ReadFromJsonAsync<UserDTO>();
                    return userProfile;
                }
                else
                {
                    _logger.LogError($"Failed to fetch user profile. Status code: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during get profile");
                ViewData["Error"] = "An error occurred";
                return null;
            }
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                var userProfile = await GetUserProfileAsync(token);
                if (userProfile != null)
                {
                    ViewData["UserProfile"] = userProfile;
                }
            }

            await base.OnActionExecutionAsync(context, next);
        }
    }

}
