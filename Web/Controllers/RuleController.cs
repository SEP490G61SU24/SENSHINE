using Microsoft.AspNetCore.Mvc;
using API.Dtos;
using System.Text;
using System.Text.Json;
using API.Ultils;
using Web.Models;
using System.Net;

namespace Web.Controllers
{
	public class RuleController : BaseController
	{
		private readonly IConfiguration _configuration;
		private readonly IHttpClientFactory _clientFactory;
		private readonly ILogger<RuleController> _logger;
		private readonly IEnumerable<string> iconList = new List<string>{"fa fa-adjust", "fa fa-anchor", "fa fa-archive", "fa fa-arrows", "fa fa-arrows-h", "fa fa-arrows-v", "fa fa-asterisk", "fa fa-automobile", "fa fa-ban", "fa fa-bank", "fa fa-bar-chart-o", "fa fa-barcode", "fa fa-bars", "fa fa-beer", "fa fa-bell", "fa fa-bell-o", "fa fa-bolt", "fa fa-bomb", "fa fa-book", "fa fa-bookmark", "fa fa-bookmark-o", "fa fa-briefcase", "fa fa-bug", "fa fa-building", "fa fa-building-o", "fa fa-bullhorn", "fa fa-bullseye", "fa fa-cab", "fa fa-calendar", "fa fa-calendar-o", "fa fa-camera", "fa fa-camera-retro", "fa fa-car", "fa fa-caret-square-o-down", "fa fa-caret-square-o-left", "fa fa-caret-square-o-right", "fa fa-caret-square-o-up", "fa fa-certificate", "fa fa-check", "fa fa-check-circle", "fa fa-check-circle-o", "fa fa-check-square", "fa fa-check-square-o", "fa fa-child", "fa fa-circle", "fa fa-circle-o", "fa fa-circle-o-notch", "fa fa-circle-thin", "fa fa-clock-o", "fa fa-cloud", "fa fa-cloud-download", "fa fa-cloud-upload", "fa fa-code", "fa fa-code-fork", "fa fa-coffee", "fa fa-cog", "fa fa-cogs", "fa fa-comment", "fa fa-comment-o", "fa fa-comments", "fa fa-comments-o", "fa fa-compass", "fa fa-credit-card", "fa fa-crop", "fa fa-crosshairs", "fa fa-cube", "fa fa-cubes", "fa fa-cutlery", "fa fa-dashboard", "fa fa-database", "fa fa-desktop", "fa fa-dot-circle-o", "fa fa-download", "fa fa-edit", "fa fa-ellipsis-h", "fa fa-ellipsis-v", "fa fa-envelope", "fa fa-envelope-o", "fa fa-envelope-square", "fa fa-eraser", "fa fa-exchange", "fa fa-exclamation", "fa fa-exclamation-circle", "fa fa-exclamation-triangle", "fa fa-external-link", "fa fa-external-link-square", "fa fa-eye", "fa fa-eye-slash", "fa fa-fax", "fa fa-female", "fa fa-fighter-jet", "fa fa-file-archive-o", "fa fa-file-audio-o", "fa fa-file-code-o", "fa fa-file-excel-o", "fa fa-file-image-o", "fa fa-file-movie-o", "fa fa-file-pdf-o", "fa fa-file-photo-o", "fa fa-file-picture-o", "fa fa-file-powerpoint-o", "fa fa-file-sound-o", "fa fa-file-video-o", "fa fa-file-word-o", "fa fa-file-zip-o", "fa fa-film", "fa fa-filter", "fa fa-fire", "fa fa-fire-extinguisher", "fa fa-flag", "fa fa-flag-checkered", "fa fa-flag-o", "fa fa-flash", "fa fa-flask", "fa fa-folder", "fa fa-folder-o", "fa fa-folder-open", "fa fa-folder-open-o", "fa fa-frown-o", "fa fa-gamepad", "fa fa-gavel", "fa fa-gear", "fa fa-gears", "fa fa-gift", "fa fa-glass", "fa fa-globe", "fa fa-graduation-cap", "fa fa-group", "fa fa-hdd-o", "fa fa-headphones", "fa fa-heart", "fa fa-heart-o", "fa fa-history", "fa fa-home", "fa fa-image", "fa fa-inbox", "fa fa-info", "fa fa-info-circle", "fa fa-institution", "fa fa-key", "fa fa-keyboard-o", "fa fa-language", "fa fa-laptop", "fa fa-leaf", "fa fa-legal", "fa fa-lemon-o", "fa fa-level-down", "fa fa-level-up", "fa fa-life-bouy", "fa fa-life-ring", "fa fa-life-saver", "fa fa-lightbulb-o", "fa fa-location-arrow", "fa fa-lock", "fa fa-magic", "fa fa-magnet", "fa fa-mail-forward", "fa fa-mail-reply", "fa fa-mail-reply-all", "fa fa-male", "fa fa-map-marker", "fa fa-meh-o", "fa fa-microphone", "fa fa-microphone-slash", "fa fa-minus", "fa fa-minus-circle", "fa fa-minus-square", "fa fa-minus-square-o", "fa fa-mobile", "fa fa-mobile-phone", "fa fa-money", "fa fa-moon-o", "fa fa-mortar-board", "fa fa-music", "fa fa-navicon", "fa fa-paper-plane", "fa fa-paper-plane-o", "fa fa-paw", "fa fa-pencil", "fa fa-pencil-square", "fa fa-pencil-square-o", "fa fa-phone", "fa fa-phone-square", "fa fa-photo", "fa fa-picture-o", "fa fa-plane", "fa fa-plus", "fa fa-plus-circle", "fa fa-plus-square", "fa fa-plus-square-o", "fa fa-power-off", "fa fa-print", "fa fa-puzzle-piece", "fa fa-qrcode", "fa fa-question", "fa fa-question-circle", "fa fa-quote-left", "fa fa-quote-right", "fa fa-random", "fa fa-recycle", "fa fa-refresh", "fa fa-reorder", "fa fa-reply", "fa fa-reply-all", "fa fa-retweet", "fa fa-road", "fa fa-rocket", "fa fa-rss", "fa fa-rss-square", "fa fa-search", "fa fa-search-minus", "fa fa-search-plus", "fa fa-send", "fa fa-send-o", "fa fa-share", "fa fa-share-alt", "fa fa-share-alt-square", "fa fa-share-square", "fa fa-share-square-o", "fa fa-shield", "fa fa-shopping-cart", "fa fa-sign-in", "fa fa-sign-out", "fa fa-signal", "fa fa-sitemap", "fa fa-sliders", "fa fa-smile-o", "fa fa-sort", "fa fa-sort-alpha-asc", "fa fa-sort-alpha-desc", "fa fa-sort-amount-asc", "fa fa-sort-amount-desc", "fa fa-sort-asc", "fa fa-sort-desc", "fa fa-sort-down", "fa fa-sort-numeric-asc", "fa fa-sort-numeric-desc", "fa fa-sort-up", "fa fa-space-shuttle", "fa fa-spinner", "fa fa-spoon", "fa fa-square", "fa fa-square-o", "fa fa-star", "fa fa-star-half", "fa fa-star-half-empty", "fa fa-star-half-full", "fa fa-star-half-o", "fa fa-star-o", "fa fa-suitcase", "fa fa-sun-o", "fa fa-support", "fa fa-tablet", "fa fa-tachometer", "fa fa-tag", "fa fa-tags", "fa fa-tasks", "fa fa-taxi", "fa fa-terminal", "fa fa-thumb-tack", "fa fa-thumbs-down", "fa fa-thumbs-o-down", "fa fa-thumbs-o-up", "fa fa-thumbs-up", "fa fa-ticket", "fa fa-times", "fa fa-times-circle", "fa fa-times-circle-o", "fa fa-tint", "fa fa-toggle-down", "fa fa-toggle-left", "fa fa-toggle-right", "fa fa-toggle-up", "fa fa-trash-o", "fa fa-tree", "fa fa-trophy", "fa fa-truck", "fa fa-umbrella", "fa fa-university", "fa fa-unlock", "fa fa-unlock-alt", "fa fa-unsorted", "fa fa-upload", "fa fa-user", "fa fa-users", "fa fa-video-camera", "fa fa-volume-down", "fa fa-volume-off", "fa fa-volume-up", "fa fa-warning", "fa fa-wheelchair", "fa fa-wrench"};

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

        [HttpGet]
        public async Task<IActionResult> Add()
		{
			try
			{
				var apiUrl = _configuration["ApiUrl"];
				using var client = _clientFactory.CreateClient();
				var response = await client.GetAsync($"{apiUrl}/rules/exclude/0");

				if (response.IsSuccessStatusCode)
				{
					RuleEditViewModel model = new RuleEditViewModel
					{
						parentRulesDTO = await response.Content.ReadFromJsonAsync<IEnumerable<RuleDTO>>(),
						ruleDTO = new RuleDTO(),
						iconList = iconList,
					};

                    return View(model);
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

		[HttpPost]
		public async Task<IActionResult> Add(RuleEditViewModel model)
		{
			try
			{
                var apiUrl = _configuration["ApiUrl"];
                using var client = _clientFactory.CreateClient();

				RuleDTO data = model.ruleDTO;

                var json = JsonSerializer.Serialize(data);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

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
				var responseRule = await client.GetAsync($"{apiUrl}/rules/{id}");
                var responseParent = await client.GetAsync($"{apiUrl}/rules/exclude/{id}");

                if (!responseParent.IsSuccessStatusCode)
                {
                    return View("Error");
                }

                if (!responseRule.IsSuccessStatusCode)
				{
                    return View("Error");
                }

				RuleEditViewModel model = new RuleEditViewModel
				{
					parentRulesDTO = await responseParent.Content.ReadFromJsonAsync<IEnumerable<RuleDTO>>(),
					ruleDTO = await responseRule.Content.ReadFromJsonAsync<RuleDTO>(),
					iconList = iconList,
				};

                return View(model);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error during login");
				ViewData["Error"] = "An error occurred";
				return View("Error");
			}
		}

		[HttpPost]
		public async Task<IActionResult> Edit(RuleEditViewModel model)
		{
			try
			{
				var apiUrl = _configuration["ApiUrl"];
				
				var data = model.ruleDTO;
				var json = JsonSerializer.Serialize(data);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				using var client = _clientFactory.CreateClient();
				var response = await client.PutAsync($"{apiUrl}/rules/{data.Id}", content);

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
					return RedirectToAction("Edit", "Rule", new {id = data.Id});
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
