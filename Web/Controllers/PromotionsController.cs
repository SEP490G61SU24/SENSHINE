using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Models;

namespace Web.Controllers
{
    public class PromotionsController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5297/api");
        private readonly HttpClient _httpClient;
        public PromotionsController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }
        [HttpGet]
        public async Task<IActionResult> ListPromotion()
        {
            List<PromotionViewModel> viewList = new List<PromotionViewModel>();
            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/ListAllPromotion");

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
