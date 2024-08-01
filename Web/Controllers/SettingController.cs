using Microsoft.AspNetCore.Mvc;
using System.Text;
using Web.Models;
using System.Text.Json;

namespace Web.Controllers
{
	public class SettingController : BaseController
	{
		private readonly IConfiguration _configuration;
		private readonly IHttpClientFactory _clientFactory;
		private readonly ILogger<SettingController> _logger;

		public SettingController(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<SettingController> logger)
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
			var response = await client.GetAsync($"{apiUrl}/systemsettings");
			if (response.IsSuccessStatusCode)
			{
				var datas = await response.Content.ReadFromJsonAsync<IEnumerable<SettingViewModel>>();
				return View(datas);
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
		public async Task<IActionResult> Add(SettingViewModel setting)
		{
			try
			{
				var apiUrl = _configuration["ApiUrl"];

				var json = JsonSerializer.Serialize(setting);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				using var client = _clientFactory.CreateClient();
				var response = await client.PostAsync($"{apiUrl}/systemsettings", content);

				if (response.IsSuccessStatusCode)
				{
					var responseString = await response.Content.ReadAsStringAsync();
					var responseData = JsonSerializer.Deserialize<SettingViewModel>(responseString);

					return RedirectToAction("Index", "Setting");
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
				ViewData["Error"] = "Có lỗi xảy ra!";
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
				var response = await client.GetAsync($"{apiUrl}/systemsettings/{id}");
				if (response.IsSuccessStatusCode)
				{
					var user = await response.Content.ReadFromJsonAsync<SettingViewModel>();
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
		public async Task<IActionResult> Edit(SettingViewModel user)
		{
			try
			{
				var apiUrl = _configuration["ApiUrl"];

				var json = JsonSerializer.Serialize(user);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				using var client = _clientFactory.CreateClient();
				var response = await client.PutAsync($"{apiUrl}/systemsettings/{user.Id}", content);

				if (response.IsSuccessStatusCode)
				{
					var responseString = await response.Content.ReadAsStringAsync();
					var responseData = JsonSerializer.Deserialize<SettingViewModel>(responseString);

					return RedirectToAction("Index", "Setting");
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
				ViewData["Error"] = "Có lỗi xảy ra!";
				return View();
			}
		}

		[HttpDelete]
		public async Task<IActionResult> Delete(string id)
		{
			try
			{
				var apiUrl = _configuration["ApiUrl"];

				using var client = _clientFactory.CreateClient();
				var response = await client.DeleteAsync($"{apiUrl}/systemsettings/{id}");

				if (response.IsSuccessStatusCode)
				{
					ViewData["SuccessMsg"] = "Xóa thành công!";
					return RedirectToAction("Index", "Setting");
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
				ViewData["Error"] = "Có lỗi xảy ra!";
				return View();
			}
		}
	}
}
