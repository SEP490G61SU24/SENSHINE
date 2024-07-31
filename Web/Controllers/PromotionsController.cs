using API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Text;
using Web.Models;

namespace Web.Controllers
{
    public class PromotionsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public PromotionsController(IConfiguration configuration)
        {
            _configuration = configuration;
            var apiUrl = _configuration.GetValue<string>("ApiUrl");
            _httpClient = new HttpClient { BaseAddress = new Uri(apiUrl) };
        }
        [HttpGet]
        public async Task<IActionResult> ListPromotion()
        {
            List<PromotionViewModel> viewList = new List<PromotionViewModel>();
            HttpResponseMessage response = await _httpClient.GetAsync("/api/ListAllPromotion");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                viewList = JsonConvert.DeserializeObject<List<PromotionViewModel>>(data);
            }

            ViewData["Title"] = "ListPromotion";

            return View(viewList);
        }
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            List<BranchViewModel> viewList = new List<BranchViewModel>();
            HttpResponseMessage response = await _httpClient.GetAsync("/api/Branch/GetAll");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                viewList = JsonConvert.DeserializeObject<List<BranchViewModel>>(data);
            }

            // Create a SelectList to populate the dropdown
            ViewBag.SpaList = new SelectList(viewList, "Id", "SpaName");

            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Add(PromotionDTORequest promotionDto)
        {
            if (!ModelState.IsValid)
            {
                return View(promotionDto);
            }

            string json = JsonConvert.SerializeObject(promotionDto);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync("/api/AddPromotion", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListPromotion");
            }

            // Log error message here
            ModelState.AddModelError(string.Empty, "An error occurred while adding the news.");
            return View(promotionDto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/api/GetPromotionDetail/{id}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var promotion = JsonConvert.DeserializeObject<PromotionViewModel>(data);

                if (promotion != null)
                {
                    // Map PromotionViewModel to your view model if necessary
                    var viewModel = new PromotionViewModel
                    {
                        Id = promotion.Id,
                        PromotionName = promotion.PromotionName,
                        Description = promotion.Description,
                        StartDate = promotion.StartDate,
                        EndDate = promotion.EndDate,
                        DiscountPercentage = promotion.DiscountPercentage,
                        SpaName = promotion.SpaName
                    };

                    return View(viewModel);
                }
            }

            return RedirectToAction("ListPromotion");
        }


        [HttpPost]
        public async Task<IActionResult> EditPromotion(PromotionViewModel promotionViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(promotionViewModel); // Return view with validation errors.
            }

            try
            {
                var promotionDto = new PromotionDTO
                {
                    Id = promotionViewModel.Id,
                    SpaId = promotionViewModel.SpaId,
                    PromotionName = promotionViewModel.PromotionName,
                    StartDate = promotionViewModel.StartDate,
                    EndDate = promotionViewModel.EndDate,
                    Description = promotionViewModel.Description,
                    DiscountPercentage = promotionViewModel.DiscountPercentage
                };

                var content = new StringContent(JsonConvert.SerializeObject(promotionDto), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PutAsync($"/api/EditPromotion/{promotionDto.Id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ListPromotion"); // Redirect to list view on success.
                }

                var errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"An error occurred: {errorMessage}"); // Add server error message.
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Exception occurred: {ex.Message}"); // Handle unexpected exceptions.
            }

            return View(promotionViewModel); // Return view with validation errors.
        }


        [HttpGet]
        public async Task<IActionResult> GetPromotionDetail(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/api/GetPromotionDetail/{id}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var promotion = JsonConvert.DeserializeObject<PromotionViewModel>(data);

                return Json(new
                {
                    id = promotion?.Id,
                    promotionName = promotion?.PromotionName,
                    description = promotion?.Description,
                    startDate = promotion?.StartDate,
                    endDate = promotion?.EndDate,
                    discountPercentage = promotion?.DiscountPercentage,
                    spaName = promotion?.SpaName
                });
            }

            return NotFound();
        }


        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/api/GetPromotionDetail/{id}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var promotionDto = JsonConvert.DeserializeObject<PromotionDTO>(data);
                return View(promotionDto);
            }

            return RedirectToAction("ListPromotion");
        }
        [HttpGet]
        public async Task<IActionResult> ListByFilter(string spaLocation, DateTime? startDate, DateTime? endDate)
        {
            // Construct the query string based on the parameters provided
            var query = new List<string>();

            if (!string.IsNullOrEmpty(spaLocation))
            {
                query.Add($"spaLocation={Uri.EscapeDataString(spaLocation)}");
            }

            if (startDate.HasValue)
            {
                query.Add($"startDate={startDate.Value:yyyy-MM-ddTHH:mm:ss}");
            }

            if (endDate.HasValue)
            {
                query.Add($"endDate={endDate.Value:yyyy-MM-ddTHH:mm:ss}");
            }

            var queryString = string.Join("&", query);
            var url = $"http://localhost:5297/api/GetPromotionDetail?{queryString}";

            HttpResponseMessage response = await _httpClient.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var promotions = JsonConvert.DeserializeObject<IEnumerable<PromotionDTORequest>>(data);
                return Json(promotions);
            }

            return StatusCode((int)response.StatusCode, "An error occurred while fetching promotions by filter.");
        }


        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/DeletePromotion/{id}");

                if (response.IsSuccessStatusCode)
                {
                
                    return Json(new { success = true });
                }

                
                return Json(new { success = false, message = "An error occurred while deleting the promotion." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An unexpected error occurred." });
            }
        }

    }
}

