using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using System.Collections.Generic;
using Web.Models;
using Newtonsoft.Json;

namespace Web.Controllers
{
    public class CategoryController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<UserController> _logger;


        public CategoryController(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<UserController> logger) : base(configuration, clientFactory, logger)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            List<CategoryViewModel> categoryList = new List<CategoryViewModel>();
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/ListAllCategory");

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

            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            if (ModelState.IsValid)
            {
                var response = await client.PostAsJsonAsync($"{apiUrl}/AddCategory", category);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(category);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            var category = await client.GetFromJsonAsync<CategoryViewModel>( $"{apiUrl}/GetCategoryDetailById/{id}");
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, CategoryViewModel category)
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            if (ModelState.IsValid)
            {
                var response = await client.PutAsJsonAsync($"{apiUrl}/EditCategory/{id}", category);
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
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            var response = await client.DeleteAsync($"{apiUrl}/DeleteCategory/{id}");
            if (response.IsSuccessStatusCode)
            {
                return Json(new { success = true });
            }
            return Json(new { success = false, message = "An error occurred while deleting the category." });
        }
    }
}
