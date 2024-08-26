using API.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using Web.Models;

namespace Web.Controllers
{
    public class RoleController: BaseController
	{
		private readonly IConfiguration _configuration;
		private readonly IHttpClientFactory _clientFactory;
		private readonly ILogger<RoleController> _logger;

		public RoleController(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<RoleController> logger)
			 : base(configuration, clientFactory, logger)
		{
			_configuration = configuration;
			_clientFactory = clientFactory;
			_logger = logger;
		}

		public async Task<IActionResult> Index()
		{
			try
			{
				var apiUrl = _configuration["ApiUrl"];
				var client = _clientFactory.CreateClient();
				var response = await client.GetAsync($"{apiUrl}/roles");
				if (response.IsSuccessStatusCode)
				{
					var data = await response.Content.ReadFromJsonAsync<IEnumerable<RoleDTO>>();
					return View(data);
				}
				else
				{
					return View("Error");
				}
			}
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }

		public IActionResult Add()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Add(RoleDTO data)
		{
			try
			{
				var apiUrl = _configuration["ApiUrl"];

				var json = JsonSerializer.Serialize(data);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				using var client = _clientFactory.CreateClient();
				var response = await client.PostAsync($"{apiUrl}/roles", content);

				if (response.IsSuccessStatusCode)
				{
					var responseString = await response.Content.ReadAsStringAsync();
					var responseData = JsonSerializer.Deserialize<RoleDTO>(responseString);

					return RedirectToAction("Index", "Role");
				}
				else
				{
					ViewData["Error"] = "Có lỗi xảy ra!";
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
		public async Task<IActionResult> Edit(int id)
		{
			try
			{
				var apiUrl = _configuration["ApiUrl"];
				var client = _clientFactory.CreateClient();

				var roleResponse = await client.GetAsync($"{apiUrl}/roles/{id}");
				if (!roleResponse.IsSuccessStatusCode)
				{
					return View("Error");
				}
				var roleData = await roleResponse.Content.ReadFromJsonAsync<RoleDTO>();

				// Lấy tất cả rules
				var rulesResponse = await client.GetAsync($"{apiUrl}/rules/all");
				if (!rulesResponse.IsSuccessStatusCode)
				{
					return View("Error");
				}
				var rulesData = await rulesResponse.Content.ReadFromJsonAsync<IEnumerable<RuleDTO>>();

				// Lấy các rules theo role ID
				var roleRulesResponse = await client.GetAsync($"{apiUrl}/rules/role/{id}/");
				if (!roleRulesResponse.IsSuccessStatusCode)
				{
					return View("Error");
				}
				var roleRulesData = await roleRulesResponse.Content.ReadFromJsonAsync<IEnumerable<RuleDTO>>();

				var viewModel = new RoleEditViewModel
				{	
					RoleId = id,
					Role = roleData,
					AllRules = rulesData,
					RoleRules = roleRulesData
				};

				return View(viewModel);
			}
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(RoleEditViewModel model)
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];
				using var client = _clientFactory.CreateClient();

				//var roleJson = JsonSerializer.Serialize(new
				//{
				//    Id = model.RoleId,
				//});
				//var roleContent = new StringContent(roleJson, Encoding.UTF8, "application/json");

				//var roleResponse = await client.PutAsync($"{apiUrl}/roles/{model.RoleId}", roleContent);

				//if (!roleResponse.IsSuccessStatusCode)
				//{
				//    ViewData["Error"] = "Có lỗi xảy ra khi cập nhật role!";
				//    return View(model);
				//}

				if(model.SelectedRuleIds == null)
				{
					model.SelectedRuleIds = new List<int>();
				}

				var ruleJson = JsonSerializer.Serialize(model.SelectedRuleIds);
                var ruleContent = new StringContent(ruleJson, Encoding.UTF8, "application/json");

                var ruleResponse = await client.PutAsync($"{apiUrl}/roles/{model.RoleId}/rules", ruleContent);

                if (ruleResponse.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index", "Role");
                }
                else
                {
                    ViewData["Error"] = "Có lỗi xảy ra khi cập nhật các rules!";
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

        [HttpDelete]
		public async Task<IActionResult> Delete(string id)
		{
			try
			{
				var apiUrl = _configuration["ApiUrl"];
				using var client = _clientFactory.CreateClient();
				var response = await client.DeleteAsync($"{apiUrl}/roles/{id}");

				if (response.IsSuccessStatusCode)
				{
					ViewData["SuccessMsg"] = "Xóa thành công!";
					return RedirectToAction("Index", "Role");
				}
				else
				{
                    ViewData["Error"] = "CÓ LỖI XẢY RA!";
                    return View("Error");
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
