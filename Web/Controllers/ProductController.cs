using API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
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
        private async Task<List<CategoryViewModel>> LoadCategoriesAsync()
        {
            List<CategoryViewModel> categories = new List<CategoryViewModel>();
            HttpResponseMessage categoryResponse = await _httpClient.GetAsync("/api/ListAllCategory");

            if (categoryResponse.IsSuccessStatusCode)
            {
                string categoryData = await categoryResponse.Content.ReadAsStringAsync();
                categories = JsonConvert.DeserializeObject<List<CategoryViewModel>>(categoryData);
            }
            else
            {
                // Handle error (e.g., log it)
                ModelState.AddModelError(string.Empty, "An error occurred while fetching categories.");
            }

            return categories;
        }


        [HttpGet]
        public async Task<IActionResult> ProductList()
        {
            List<ProductViewModel> viewList = new List<ProductViewModel>();
            List<CategoryViewModel> categoryList = await LoadCategoriesAsync();

            HttpResponseMessage response = await _httpClient.GetAsync("/api/ListAllProduct");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                viewList = JsonConvert.DeserializeObject<List<ProductViewModel>>(data);
            }
            else
            {
                // Handle error (e.g., log it)
                ModelState.AddModelError(string.Empty, "An error occurred while fetching products.");
            }

            // Store categories in ViewBag
            ViewBag.Categories = categoryList;

            ViewData["Title"] = "List Products";
            return View(viewList);
        }

        [HttpGet]
        public async Task<IActionResult> FilterProducts(string categoryName, string quantityRange, string priceRange)
        {
            // Build the query string for filtering products
            var queryParameters = new List<string>();

            if (!string.IsNullOrEmpty(categoryName))
            {
                queryParameters.Add($"categoryName={Uri.EscapeDataString(categoryName)}");
            }
            if (!string.IsNullOrEmpty(quantityRange))
            {
                queryParameters.Add($"quantityRange={Uri.EscapeDataString(quantityRange)}");
            }
            if (!string.IsNullOrEmpty(priceRange))
            {
                queryParameters.Add($"priceRange={Uri.EscapeDataString(priceRange)}");
            }

            string queryString = string.Join("&", queryParameters);
            string requestUri = $"/api/GetFilterProducts?{queryString}";

            HttpResponseMessage response = await _httpClient.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var productList = JsonConvert.DeserializeObject<IEnumerable<ProductViewModel>>(data);
                return Json(productList);
            }

            return Json(new List<ProductViewModel>());
        }


        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var categories = await LoadCategoriesAsync();
            ViewBag.Categories = categories ?? new List<CategoryViewModel>();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(ProductDTORequest_2 productDto)
        {
            var content = new StringContent(JsonConvert.SerializeObject(productDto), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync("/api/AddProduct", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ProductList");
            }

            // Read the response content for error details
            string errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"An error occurred while adding the product: {errorMessage}");
            return View(productDto);
        }


        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/api/GetProductDetail/{id}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var productDto = JsonConvert.DeserializeObject<ProductDTORequest_2>(data);
                await LoadCategoriesAsync();
                return View(productDto);
            }

            // Log the error or handle it as necessary
            string errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"An error occurred while fetching the product details: {errorMessage}");
            return RedirectToAction("ProductList");
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProductDTORequest_2 productDto)
        {
            var content = new StringContent(JsonConvert.SerializeObject(productDto), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PutAsync($"/api/EditProduct/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ProductList");
            }

            // Read the response content for error details
            string errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"An error occurred while editing the product: {errorMessage}");
            return View(productDto);
        }


        [HttpGet]
        public async Task<IActionResult> GetProductDetail(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/api/GetProductDetailById/{id}");

            if (response.IsSuccessStatusCode)
            {
               
                string data = await response.Content.ReadAsStringAsync();
                var product = JsonConvert.DeserializeObject<ProductDTORequest>(data);

               
                if (product == null)
                {
                    return NotFound();
                }

               
                var result = new
                {
                    id = product.Id,
                    name = product.ProductName,
                    price = product.Price,
                    quantity = product.Quantity,
                    categories = product.Categories?.Select(c => c.CategoryName) 
                };

                return Json(result);
            }

            
            return NotFound();
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

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/DeleteProduct/{id}");

                if (response.IsSuccessStatusCode)
                {

                    return Json(new { success = true });
                }


                return Json(new { success = false, message = "An error occurred while deleting the product." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An unexpected error occurred." });
            }
        }

    }

}
