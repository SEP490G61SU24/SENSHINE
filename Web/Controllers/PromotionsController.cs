using API.Dtos;
using Microsoft.AspNetCore.Mvc;
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
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddPromotion(PromotionDTO promotionDto)
        {
            var content = new StringContent(JsonConvert.SerializeObject(promotionDto), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync("/api/AddPromotion", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListPromotion");
            }

            ModelState.AddModelError(string.Empty, "An error occurred while adding the promotion.");
            return View(promotionDto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
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

        [HttpPost]
        public async Task<IActionResult> EditPromotion(int id, PromotionDTO promotionDto)
        {
            var content = new StringContent(JsonConvert.SerializeObject(promotionDto), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PutAsync($"/api/EditPromotion/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListPromotion");
            }

            ModelState.AddModelError(string.Empty, "An error occurred while editing the promotion.");
            return View(promotionDto);
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
        public async Task<IActionResult> ListByCode(string code)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/api/PromotionByCode/{code}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var promotions = JsonConvert.DeserializeObject<IEnumerable<PromotionDTORequest>>(data);
                return View(promotions);
            }

            ModelState.AddModelError(string.Empty, "An error occurred while fetching promotions by code.");
            return View(new List<PromotionDTORequest>());
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"/api/DeletePromotion/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListPromotion");
            }

            ModelState.AddModelError(string.Empty, "An error occurred while deleting the promotion.");
            return RedirectToAction("ListPromotion");
        }
    }
}

