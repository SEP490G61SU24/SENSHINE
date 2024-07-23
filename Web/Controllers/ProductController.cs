using API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Web.Models;

namespace Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public ProductController(IConfiguration configuration)
        {
            _configuration = configuration;
            var apiUrl = _configuration.GetValue<string>("ApiUrl");
            _httpClient = new HttpClient { BaseAddress = new Uri(apiUrl) };
        }

        [HttpGet]
        public async Task<IActionResult> ProductList()
        {
            List<ProductViewModel> viewList = new List<ProductViewModel>();
            HttpResponseMessage response = await _httpClient.GetAsync("/api/ListAllProduct");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                viewList = JsonConvert.DeserializeObject<List<ProductViewModel>>(data);
            }

            ViewData["Title"] = "List Products";
            return View(viewList);
        }

        [HttpGet]
        public async Task<IActionResult> CategoryList()
        {
            List<CategoryViewModel> viewList = new List<CategoryViewModel>();
            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/ListAllCategories");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                viewList = JsonConvert.DeserializeObject<List<CategoryViewModel>>(data);
            }


            return View(viewList);
        }
    
    [HttpGet]
    public IActionResult AddProduct()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddProduct(ProductDTO productDto)
    {
        var content = new StringContent(JsonConvert.SerializeObject(productDto), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _httpClient.PostAsync("/api/AddProduct", content);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("ProductList");
        }

        ModelState.AddModelError(string.Empty, "An error occurred while adding the product.");
        return View(productDto);
    }

    [HttpGet]
    public async Task<IActionResult> EditProduct(int id)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"/api/GetProductDetail/{id}");

        if (response.IsSuccessStatusCode)
        {
            string data = await response.Content.ReadAsStringAsync();
            var productDto = JsonConvert.DeserializeObject<ProductDTO>(data);
            return View(productDto);
        }

        return RedirectToAction("ProductList");
    }

    [HttpPost]
    public async Task<IActionResult> EditProduct(int id, ProductDTO productDto)
    {
        var content = new StringContent(JsonConvert.SerializeObject(productDto), Encoding.UTF8, "application/json");
        HttpResponseMessage response = await _httpClient.PutAsync($"/api/EditProduct/{id}", content);

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("ProductList");
        }

        ModelState.AddModelError(string.Empty, "An error occurred while editing the product.");
        return View(productDto);
    }

    [HttpGet]
    public async Task<IActionResult> ProductDetail(int id)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"/api/GetProductDetail/{id}");

        if (response.IsSuccessStatusCode)
        {
            string data = await response.Content.ReadAsStringAsync();
            var productDto = JsonConvert.DeserializeObject<ProductDTO>(data);
            return View(productDto);
        }

        return RedirectToAction("ProductList");
    }

    [HttpGet]
    public async Task<IActionResult> ProductsByCategory(int categoryId)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"/api/ProductsByCategory/{categoryId}");

        if (response.IsSuccessStatusCode)
        {
            string data = await response.Content.ReadAsStringAsync();
            var products = JsonConvert.DeserializeObject<IEnumerable<ProductDTO>>(data);
            return View(products);
        }

        ModelState.AddModelError(string.Empty, "An error occurred while fetching products by category.");
        return View(new List<ProductDTO>());
    }

    [HttpPost]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        HttpResponseMessage response = await _httpClient.DeleteAsync($"/api/DeleteProduct/{id}");

        if (response.IsSuccessStatusCode)
        {
            return RedirectToAction("ProductList");
        }

        ModelState.AddModelError(string.Empty, "An error occurred while deleting the product.");
        return RedirectToAction("ProductList");
    }
}
}