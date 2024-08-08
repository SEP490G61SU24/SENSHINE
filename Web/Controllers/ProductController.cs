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
        private async Task<List<CategoryViewModel>> LoadCategoriesByProductIdAsync(int productId)
        {
            List<CategoryViewModel> categories = new List<CategoryViewModel>();
            HttpResponseMessage categoryResponse = await _httpClient.GetAsync($"api/GetCategoriesByProductId?id={productId}");

            if (categoryResponse.IsSuccessStatusCode)
            {
                string categoryData = await categoryResponse.Content.ReadAsStringAsync();
                // Assuming the response is a list of CategoryDTO
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

            // Send the GET request to the API endpoint
            HttpResponseMessage response = await _httpClient.GetAsync(requestUri);

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var productList = JsonConvert.DeserializeObject<IEnumerable<ProductViewModel>>(data);
                return Json(productList);
            }

            // Handle any unsuccessful responses or errors
            return Json(new List<ProductViewModel>());
        }


        [HttpGet]
        public async Task<IActionResult> Add()
        {
            try
            {
                var categories = await LoadCategoriesAsync();
                ViewBag.Categories = categories ?? new List<CategoryViewModel>();
            }
            catch (Exception ex)
            {
               
                ModelState.AddModelError(string.Empty, "An error occurred while loading categories.");
            }

            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Add(ProductViewModel productDto)
        {
           

            try
            {
                // Convert CategoryIdsString to a list of integers
                productDto.CategoryIds = productDto.CategoryIdsString?.Split(',').Select(int.Parse).ToList();

                // Create content for the request
                var content = new StringContent(JsonConvert.SerializeObject(productDto), Encoding.UTF8, "application/json");

                // Send the POST request
                HttpResponseMessage response = await _httpClient.PostAsync("/api/AddProduct", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ProductList");
                }

                // Read the response content for error details
                string errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"An error occurred while adding the product: {errorMessage}");
            }
            catch (Exception ex)
            {
                // Log the exception
                // e.g., _logger.LogError(ex, "Failed to add product");
                ModelState.AddModelError(string.Empty, "An error occurred while adding the product.");
            }

            // Reload categories to show in the view if there's an error
            var categories = await LoadCategoriesAsync();
            ViewBag.Categories = categories ?? new List<CategoryViewModel>();

            return View(productDto);
        }



        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            
            var categories = await LoadCategoriesAsync();
            ViewBag.Categories = categories ?? new List<CategoryViewModel>();

            // Fetch product details from the API
            HttpResponseMessage response = await _httpClient.GetAsync($"/api/GetProductDetailById/{id}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var productDto = JsonConvert.DeserializeObject<ProductViewModel>(data);


                if (productDto == null)
                {
                    ModelState.AddModelError(string.Empty, "Product not found.");
                    return RedirectToAction("ProductList");
                }
                // Fetch the list of categories
                var check = await LoadCategoriesByProductIdAsync(productDto.Id);               
                var categoryIds = check.Select(c => c.Id).ToList();    
                productDto.CategoryIds = categoryIds;

                // Return the view with the product data
                return View(productDto); 
            }

            // Log the error or handle it as necessary
            string errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, $"An error occurred while fetching the product details: {errorMessage}");
            return RedirectToAction("ProductList");
        }



        [HttpPost]
        public async Task<IActionResult> Edit(int id, ProductViewModel productDto)
        {
            // Load categories for the view
            var categories = await LoadCategoriesAsync();
            ViewBag.Categories = categories ?? new List<CategoryViewModel>();

            

            productDto.CategoryIds = productDto.CategoryIdsString.Split(',').Select(int.Parse).ToList();



            var content = new StringContent(JsonConvert.SerializeObject(productDto), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PutAsync($"/api/EditProduct/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                // Redirect to the product list on successful update
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
                    categories = product.Categories?.Select(c => c.CategoryName),
                    imageUrl = product.ProductImages?.Select(c=>c.ImageUrl).FirstOrDefault()
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
