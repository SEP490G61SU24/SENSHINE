using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Headers;
using API.Dtos;
using NuGet.Protocol.Plugins;
using Microsoft.AspNetCore.Http;
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
                    UserDTO userProfile = await response.Content.ReadFromJsonAsync<UserDTO>();
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

        protected async Task<IEnumerable<MenuDTO>> GetMenuByRole(int? roleId = 1)
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];
                using var client = _clientFactory.CreateClient();

                var response = await client.GetAsync($"{apiUrl}/rules/menu/{roleId}");

                if (response.IsSuccessStatusCode)
                {
                    var data = await response.Content.ReadFromJsonAsync<IEnumerable<MenuDTO>>();
                    return data;
                }
                else
                {
                    _logger.LogError($"Failed to fetch menu. Status code: {response.StatusCode}");
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
            var spaId = HttpContext.Session.GetString("SpaId");
            var token = HttpContext.Session.GetString("Token");
			IEnumerable<MenuDTO> menus = new List<MenuDTO>();

			if (!string.IsNullOrEmpty(token))
            {
                UserDTO userProfile = await GetUserProfileAsync(token);
                if (userProfile != null)
                {
                    ViewData["UserProfile"] = userProfile;
                    menus = await GetMenuByRole(userProfile.RoleId);

                    if (string.IsNullOrEmpty(spaId) && userProfile.RoleId == (int) UserRoleEnum.CEO)
                    {
                        spaId = "ALL";
                    }
                    else if (string.IsNullOrEmpty(spaId) && userProfile.SpaId != null)
                    {
                        HttpContext.Session.SetString("SpaId", userProfile.SpaId?.ToString() ?? "ALL");
                        spaId = userProfile.SpaId?.ToString() ?? "ALL";
                    }
                }

                ViewData["Token"] = token;
            }
            
            ViewData["SpaId"] = spaId;
            ViewData["UserMenu"] = menus;

            var successMessage = TempData["SuccessMsg"];
            if (successMessage != null)
            {
                ViewData["SuccessMsg"] = successMessage;
            }
            var errorMessage = TempData["Error"];
            if (errorMessage != null)
            {
                ViewData["Error"] = errorMessage;
            }

            await base.OnActionExecutionAsync(context, next);
        }
    }

}
