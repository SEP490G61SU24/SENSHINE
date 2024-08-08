using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
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
    }

    public class LoginResponse
    {
        public int id { get; set; }
        public string username { get; set; }
        public string token { get; set; }
    }
}
