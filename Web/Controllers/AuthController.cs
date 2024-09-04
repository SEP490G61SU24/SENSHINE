using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using Web.Models;
using API.Dtos;

namespace Web.Controllers
{
    public class AuthController : BaseController
	{
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<AuthController> logger)
			: base(configuration, clientFactory, logger)
		{
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
        }
        
        [HttpGet]
        public IActionResult Login()
        {
            return RedirectToAction("LoginSpa", "Auth");
        }

        [HttpGet]
        public IActionResult LoginSpa()
        {
			UserDTO userProfile = ViewData["UserProfile"] as UserDTO;
			if (userProfile != null)
			{
				return RedirectToAction("Index", "Dashboard");
			}
			return View("Login");
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
                    var loginResponse = JsonSerializer.Deserialize<LoginResponseModel>(responseString);

                    // Lưu trữ token vào Session
                    HttpContext.Session.SetString("Token", loginResponse.token);
                    Response.Cookies.Append("Token", loginResponse.token, new CookieOptions
                    {
                        HttpOnly = true, // Chỉ có thể truy cập từ phía máy chủ
                        Secure = true,   // Chỉ sử dụng trong môi trường HTTPS
                        SameSite = SameSiteMode.Strict, // Ngăn chặn các yêu cầu từ một trang web khác
                        Expires = DateTimeOffset.UtcNow.AddHours(24) // Thời gian hết hạn của cookie
                    });

                    TempData["SuccessMsg"] = "Đăng nhập thành công!";
                    return RedirectToAction("My", "WorkSchedule");
                }
                else
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    ViewData["Error"] = responseString;
                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("Token");
            Response.Cookies.Delete("Token");
            HttpContext.Session.Remove("SpaId");

            ViewData["UserProfile"] = null;

            return RedirectToAction("LoginSpa", "Auth");
        }

        [HttpGet]
		public IActionResult ChangePass()
		{
			UserDTO userProfile = ViewData["UserProfile"] as UserDTO;
			if(userProfile == null)
            {
				return RedirectToAction("LoginSpa", "Auth");
			}
			return View();
		}

        [HttpPost]
        public IActionResult ChangeSpa(string spaId)
        {
            HttpContext.Session.SetString("SpaId", spaId);
            return RedirectToAction("Index", "Dashboard");
        }

        [HttpPost]
		public async Task<IActionResult> ChangePass(ChangePasswordDTO model)
		{
			try
			{
				UserDTO userProfile = ViewData["UserProfile"] as UserDTO;
			    if (userProfile == null)
			    {
				    return RedirectToAction("LoginSpa", "Auth");
			    }

                if (string.IsNullOrEmpty(model.OldPassword) || string.IsNullOrEmpty(model.NewPassword) || string.IsNullOrEmpty(model.RePassword))
                {
				    ViewData["Error"] = "Vui lòng điền đủ thông tin!";
				    return View(model);
			    }

                if(model.NewPassword != model.RePassword)
                {
				    ViewData["Error"] = "Mật khẩu nhập lại không khớp!";
				    return View(model);
			    }

			    var apiUrl = _configuration["ApiUrl"];
			    using var client = _clientFactory.CreateClient();

				var data = new ChangePasswordDTO
				{
			        UserName = userProfile.UserName,
					OldPassword = model.OldPassword,
					NewPassword = model.NewPassword,
		        };

				var json = JsonSerializer.Serialize(data);
			    var content = new StringContent(json, Encoding.UTF8, "application/json");
			    var response = await client.PostAsync($"{apiUrl}/auth/changepass", content);

			    if (response.IsSuccessStatusCode)
			    {
				    ViewData["SuccessMsg"] = "Đổi mật khẩu thành công!";
				    return RedirectToAction("Index", "User");
			    }
			    else
			    {
				    var responseString = await response.Content.ReadAsStringAsync();
				    ViewData["Error"] = responseString;
				    return View(model);
			    }
		    }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }

	}
}
