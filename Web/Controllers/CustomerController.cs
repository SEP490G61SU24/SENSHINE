using Microsoft.AspNetCore.Mvc;
using System.Text;
using Web.Models;
using System.Text.Json;

namespace Web.Controllers
{
	public class CustomerController : BaseController
	{
		private readonly IConfiguration _configuration;
		private readonly IHttpClientFactory _clientFactory;
		private readonly ILogger<CustomerController> _logger;

		public CustomerController(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<CustomerController> logger)
			 : base(configuration, clientFactory, logger)
		{
			_configuration = configuration;
			_clientFactory = clientFactory;
			_logger = logger;
		}

		public async Task<IActionResult> Index()
		{
			var apiUrl = _configuration["ApiUrl"];
			var client = _clientFactory.CreateClient();
			var response = await client.GetAsync($"{apiUrl}/user/byRole/5");
			if (response.IsSuccessStatusCode)
			{
				var users = await response.Content.ReadFromJsonAsync<IEnumerable<UserViewModel>>();
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

		[HttpPost]
		public async Task<IActionResult> Add(UserViewModel user)
		{
			try
			{
				user.Password = "123456";

				string[] nameArr = user.FullName.Split(" ");

				if (nameArr.Length < 2)
				{
					user.FirstName = null;
					user.MidName = null;
					user.LastName = nameArr[0];
				}
				else if (nameArr.Length < 3)
				{
					user.FirstName = nameArr[0];
					user.MidName = null;
					user.LastName = nameArr[1];
				}
				else
				{
					user.FirstName = nameArr[0];
					user.MidName = nameArr[1];
					user.LastName = nameArr[nameArr.Length - 1];
				}

				user.UserName = (user.LastName + user.ProvinceCode + GenerateRandomString(4)).ToLower();
				user.RoleId = 5; // ROLE CUSTOMER

				var apiUrl = _configuration["ApiUrl"];

				var json = JsonSerializer.Serialize(user);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				using var client = _clientFactory.CreateClient();
				var response = await client.PostAsync($"{apiUrl}/user/add", content);

				if (response.IsSuccessStatusCode)
				{
					var responseString = await response.Content.ReadAsStringAsync();
					var responseData = JsonSerializer.Deserialize<UserViewModel>(responseString);

					return RedirectToAction("Index", "User");
				}
				else
				{
					ViewData["Error"] = "Có lỗi xảy ra!";
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

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();
                var response = await client.GetAsync($"{apiUrl}/user/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadFromJsonAsync<UserViewModel>();
                    user.FullName = string.Join(" ", new[] { user.FirstName, user.MidName, user.LastName }.Where(name => !string.IsNullOrEmpty(name)));
                    return View(user);
                }
                else
                {
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                ViewData["Error"] = "An error occurred";
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserViewModel user)
        {
            try
            {
                string[] nameArr = user.FullName.Split(" ");

                if (nameArr.Length < 2)
                {
                    user.FirstName = null;
                    user.MidName = null;
                    user.LastName = nameArr[0];
                }
                else if (nameArr.Length < 3)
                {
                    user.FirstName = nameArr[0];
                    user.MidName = null;
                    user.LastName = nameArr[1];
                }
                else
                {
                    user.FirstName = nameArr[0];
                    user.MidName = nameArr[1];
                    user.LastName = nameArr[nameArr.Length - 1];
                }

				user.RoleId = 5; // CUSTOMER

                var apiUrl = _configuration["ApiUrl"];

                var json = JsonSerializer.Serialize(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var client = _clientFactory.CreateClient();
                var response = await client.PutAsync($"{apiUrl}/user/update/{user.Id}", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var responseData = JsonSerializer.Deserialize<UserViewModel>(responseString);

                    //return View(responseData);
                    return RedirectToAction("Index", "Customer");
                }
                else
                {
                    ViewData["Error"] = "Có lỗi xảy ra!";
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

        public static string GenerateRandomString(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			char[] stringChars = new char[length];

			for (int i = 0; i < length; i++)
			{
				stringChars[i] = chars[new Random().Next(chars.Length)];
			}

			return new string(stringChars);
		}
	}
}
