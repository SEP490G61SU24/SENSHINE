using Microsoft.AspNetCore.Mvc;
using API.Dtos;
using System.Text;
using System.Text.Json;
using API.Ultils;

namespace Web.Controllers
{
    public class RuleController : BaseController
	{
		private readonly IConfiguration _configuration;
		private readonly IHttpClientFactory _clientFactory;
		private readonly ILogger<RuleController> _logger;

		public RuleController(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<RuleController> logger)
			 : base(configuration, clientFactory, logger)
		{
			_configuration = configuration;
			_clientFactory = clientFactory;
			_logger = logger;
		}

        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10, string searchTerm = null)
        {
            var apiUrl = _configuration["ApiUrl"];
			var client = _clientFactory.CreateClient();

            var url = $"{apiUrl}/rules?pageIndex={pageIndex}&pageSize={pageSize}&searchTerm={searchTerm}";

            var response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var paginatedResult = await response.Content.ReadFromJsonAsync<PaginatedList<RuleDTO>>();
                paginatedResult.SearchTerm = searchTerm;
                return View(paginatedResult);
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
		public async Task<IActionResult> Add(RuleDTO data)
		{
			try
			{
				var apiUrl = _configuration["ApiUrl"];

				var json = JsonSerializer.Serialize(data);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				using var client = _clientFactory.CreateClient();
				var response = await client.PostAsync($"{apiUrl}/rules", content);

				if (response.IsSuccessStatusCode)
				{
					var responseString = await response.Content.ReadAsStringAsync();
					var responseData = JsonSerializer.Deserialize<RuleDTO>(responseString);

					return RedirectToAction("Index", "Rule");
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
				var response = await client.GetAsync($"{apiUrl}/rules/{id}");
				if (response.IsSuccessStatusCode)
				{
					var data = await response.Content.ReadFromJsonAsync<RuleDTO>();
					return View(data);
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
		public async Task<IActionResult> Edit(RuleDTO body)
		{
			try
			{
				var apiUrl = _configuration["ApiUrl"];
				var json = JsonSerializer.Serialize(body);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				using var client = _clientFactory.CreateClient();
				var response = await client.PutAsync($"{apiUrl}/rules/{body.Id}", content);

				if (response.IsSuccessStatusCode)
				{
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        return RedirectToAction("Index", "Rule");
                    }

                    var responseString = await response.Content.ReadAsStringAsync();
                    var responseData = JsonSerializer.Deserialize<RuleDTO>(responseString);
                    return RedirectToAction("Index", "Rule");
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

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];
                using var client = _clientFactory.CreateClient();
                var response = await client.DeleteAsync($"{apiUrl}/rules/{id}");

                if (response.IsSuccessStatusCode)
                {
                    ViewData["SuccessMsg"] = "Xóa thành công!";
                    return RedirectToAction("Index", "Rule");
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
