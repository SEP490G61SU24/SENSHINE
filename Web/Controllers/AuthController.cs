using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Web.Models;

namespace Web.Controllers
{
    public class AuthController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<AuthController> _logger;
        public AuthController(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<AuthController> logger)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(UserLoginViewModel user)
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];

                var json = JsonSerializer.Serialize(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var client = _clientFactory.CreateClient();
                var response = await client.PostAsync($"{apiUrl}/auth/login", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var loginResponse = JsonSerializer.Deserialize<LoginResponse>(responseString);

                    // Lưu trữ token vào Session
                    HttpContext.Session.SetString("Token", loginResponse.token);
                    Response.Cookies.Append("Token", loginResponse.token, new CookieOptions
                    {
                        HttpOnly = true, // Chỉ có thể truy cập từ phía máy chủ
                        Secure = true,   // Chỉ sử dụng trong môi trường HTTPS
                        SameSite = SameSiteMode.Strict, // Ngăn chặn các yêu cầu từ một trang web khác
                        Expires = DateTimeOffset.UtcNow.AddHours(24) // Thời gian hết hạn của cookie
                    });

                    return RedirectToAction("Index", "User");
                }
                else
                {
                    ViewData["Error"] = "Invalid username or password";
                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                ViewData["Error"] = "An error occurred";
                return View();
            }
        }

        [HttpPost]
        public async Task<IActionResult> Register(string username, string password, string firstName, string lastName)
        {
            try
            {
                var registerRequest = new
                {
                    UserName = username,
                    Password = password,
                    FirstName = firstName,
                    LastName = lastName
                };

                var apiUrl = _configuration["ApiUrl"]; // Lấy URL của API từ cấu hình

                var json = JsonSerializer.Serialize(registerRequest);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var client = _clientFactory.CreateClient();
                var response = await client.PostAsync($"{apiUrl}/auth/register", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Home"); // Chuyển hướng tới trang chính
                }
                else
                {
                    ViewData["Error"] = "Registration failed";
                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during registration");
                ViewData["Error"] = "An error occurred";
                return View();
            }
        }
    }

    public class LoginResponse
    {
        public int id { get; set; }
        public string username { get; set; }
        public string token { get; set; }
    }
}
