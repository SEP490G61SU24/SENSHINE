using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        public IActionResult Add()
        {
            return View();
        }
    }
}
