using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Web.Models;
using Newtonsoft.Json;

namespace Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly HttpClient _httpClient;

        public CategoryController()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5297/api") };
        }

        public async Task<IActionResult> Index()
        {
            List<CategoryViewModel> categoryList = new List<CategoryViewModel>();
            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/ListAllCategory");

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                categoryList = JsonConvert.DeserializeObject<List<CategoryViewModel>>(jsonString);
            }
            return View(categoryList);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(CategoryViewModel category)
        {
            if (ModelState.IsValid)
            {
                var response = await _httpClient.PostAsJsonAsync(_httpClient.BaseAddress + "/AddCategory", category);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(category);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var category = await _httpClient.GetFromJsonAsync<CategoryViewModel>(_httpClient.BaseAddress + $"/GetCategoryDetailById/{id}");
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CategoryViewModel category)
        {
            if (ModelState.IsValid)
            {
                var response = await _httpClient.PutAsJsonAsync(_httpClient.BaseAddress + $"/EditCategory/{id}", category);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(category);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var response = await _httpClient.DeleteAsync(_httpClient.BaseAddress + $"/DeleteCategory/{id}");
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "An error occurred while deleting the category." });
        }
    }
}
