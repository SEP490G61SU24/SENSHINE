using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Models;

namespace Web.Controllers
{
    public class ReviewController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5297/api");
        private readonly HttpClient _httpClient;
        public ReviewController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchString)
        {
            List<ReviewViewModel> reviewList = new List<ReviewViewModel>();

            try
            {
                // Gọi API để lấy danh sách dịch vụ
                HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/Review/GetAllReview");

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    reviewList = JsonConvert.DeserializeObject<List<ReviewViewModel>>(data);

                    
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khi gọi API
                ModelState.AddModelError(string.Empty, $"Có lỗi xảy ra: {ex.Message}");
            }

            return View(reviewList);
        }
    }
}
