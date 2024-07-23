using API.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Web.Models;

namespace Web.Controllers
{
    public class ComboController : Controller
    {
        private readonly HttpClient _httpClient;

        public ComboController()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5297/api") };
        }

        // Hiển thị danh sách combo
        public async Task<IActionResult> Index()
        {
            List<ComboViewModel> comboList = new List<ComboViewModel>();
            //HttpResponseMessage response = await _httpClient.GetAsync("/Combo/GetAllCombo");
            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/Combo/GetAllCombo");

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                comboList = JsonSerializer.Deserialize<List<ComboViewModel>>(jsonString);
            }
            return View(comboList);
        }
    }
}
