using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Json;
using API.Models;
using System.Configuration;
using Web.Models;
using System.Net.Http.Headers;
using System.Text.Json;

namespace Web.Controllers
{
    public class UserController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<UserController> _logger;

        public UserController(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<UserController> logger)
             : base(configuration, clientFactory, logger)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public async Task<UserViewModel> GetUserProfileAsync(string token)
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];

                using var client = _clientFactory.CreateClient();
                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await client.GetAsync($"{apiUrl}/user/profile");

                if (response.IsSuccessStatusCode)
                {
                    var jsonString = await response.Content.ReadAsStringAsync();
                    var userProfile = JsonSerializer.Deserialize<UserViewModel>(jsonString);

                    //var userViewModel = new UserViewModel
                    //{
                    //    UserId = userProfile.UserId,
                    //    Username = userProfile.Username,
                    //};

                    return userProfile;
                }
                else
                {
                    _logger.LogError($"Failed to fetch user profile. Status code: {response.StatusCode}");
                    return null; // or throw new custom exception if needed
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during get profile");
                ViewData["Error"] = "An error occurred"; // Example: Set ViewData for error message
                return null; // or throw a custom exception if needed
            }
        }

        public async Task<IActionResult> Index()
        {
            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync("http://localhost:5297/api/user/all");
            if (response.IsSuccessStatusCode)
            {
                var users = await response.Content.ReadFromJsonAsync<IEnumerable<UserDto>>();
                return View(users);
            }
            else
            {
                return View("Error");
            }
        }

        public IActionResult Add()
        {
            return View();
        }
    }

    public class UserDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string? FirstName { get; set; }
        public string? MidName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? ProvinceCode { get; set; }
        public string? DistrictCode { get; set; }
        public string? WardCode { get; set; }
    }
}
