using API.Dtos;
using API.Ultils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Web.Models;

namespace Web.Controllers
{
    public class ProductController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<UserController> _logger;


        public ProductController(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<UserController> logger) : base(configuration, clientFactory, logger)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
        }
        private async Task<UserViewModel> LoadUserAsync()
        {
            var user = new UserViewModel();
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                var userProfile = await GetUserProfileAsync(token);

                if (userProfile != null)
                {
                    user.Id = userProfile.Id;
                    user.UserName = userProfile.UserName;
                    user.FirstName = userProfile.FirstName;
                    user.MidName = userProfile.MidName;
                    user.LastName = userProfile.LastName;
                    user.Phone = userProfile.Phone;
                    user.BirthDate = userProfile.BirthDate;
                    user.Status = userProfile.Status;
                    user.StatusWorking = userProfile.StatusWorking;
                    user.SpaId = userProfile.SpaId;
                    user.ProvinceCode = userProfile.ProvinceCode;
                    user.DistrictCode = userProfile.DistrictCode;
                    user.WardCode = userProfile.WardCode;
                    user.Address = userProfile.Address;
                    user.Roles = userProfile.Roles;
                    user.RoleName = userProfile.RoleName;
                    user.RoleId = userProfile.RoleId;
                    user.FullName = $"{userProfile.FirstName} {userProfile.MidName} {userProfile.LastName}";
                }
                else
                {
                    ViewData["Error"] = "Failed to retrieve user profile.";
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An error occurred while fetching the user profile.");
            }

            return user;
        }
        private async Task<List<CategoryViewModel>> LoadCategoriesAsync()
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            List<CategoryViewModel> categories = new List<CategoryViewModel>();
            HttpResponseMessage categoryResponse = await client.GetAsync($"{apiUrl}/ListAllCategory");

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
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            List<CategoryViewModel> categories = new List<CategoryViewModel>();
            HttpResponseMessage categoryResponse = await client.GetAsync($"{apiUrl}/GetCategoriesByProductId?id={productId}");

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
        public async Task<IActionResult> ProductList(int? spaId = null, int pageIndex = 1, int pageSize = 10, string searchTerm = null, string? categoryName = null, string? quantityRange = null, string? priceRange = null)
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            var use = await LoadUserAsync();
            List<CategoryViewModel> categoryList = await LoadCategoriesAsync();
            spaId = use.SpaId;
            var urlBuilder = new StringBuilder($"{apiUrl}/GetProductsPaging?");

            if (spaId != null)
            {
                urlBuilder.Append($"spaId={spaId}&");
            }

            if (categoryName != null)
            {
                urlBuilder.Append($"categoryName={categoryName}&");
            }

            if (quantityRange != null)
            {
                urlBuilder.Append($"quantityRange={quantityRange}&");
            }
            if (priceRange != null)
            {
                urlBuilder.Append($"priceRange={priceRange}&");
            }
            urlBuilder.Append($"pageIndex={pageIndex}&pageSize={pageSize}&searchTerm={searchTerm}");

            // Remove the trailing '&' if it exists
            var url = urlBuilder.ToString().TrimEnd('&');

            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var paginatedResult = await response.Content.ReadFromJsonAsync<PaginatedList<ProductViewModel>>();
                paginatedResult.SearchTerm = searchTerm;
                ViewBag.Categories = categoryList;
                return View(paginatedResult);
            }
            else
            {
                return View("Error");
            }
        }

        

        [HttpGet]
        public async Task<IActionResult> FilterProducts(string categoryName, string quantityRange, string priceRange)
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
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
            string requestUri = $"{apiUrl}/GetFilterProducts?{queryString}";

            // Send the GET request to the API endpoint
            HttpResponseMessage response = await client.GetAsync(requestUri);

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
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
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
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            var user = await LoadUserAsync();
            try
            {
                // Convert CategoryIdsString to a list of integers
                productDto.CategoryIds = productDto.CategoryIdsString?.Split(',').Select(int.Parse).ToList();
                if (user.SpaId == null)
                {
                    int id = 1;
                    id = productDto.SpaId;
                }
                else
                {
                    user.SpaId = productDto.SpaId;
                }
                // Create content for the request
                var content = new StringContent(JsonConvert.SerializeObject(productDto), Encoding.UTF8, "application/json");

                // Send the POST request
                HttpResponseMessage response = await client.PostAsync($"{apiUrl}/AddProduct", content);

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
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            var categories = await LoadCategoriesAsync();
            ViewBag.Categories = categories ?? new List<CategoryViewModel>();

            // Fetch product details from the API
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/GetProductDetailById/{id}");

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
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            // Load categories for the view
            var categories = await LoadCategoriesAsync();
            ViewBag.Categories = categories ?? new List<CategoryViewModel>();

            

            productDto.CategoryIds = productDto.CategoryIdsString.Split(',').Select(int.Parse).ToList();



            var content = new StringContent(JsonConvert.SerializeObject(productDto), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync($"{apiUrl}/EditProduct/{id}", content);

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
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/GetProductDetailById/{id}");

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
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/ProductsByCategory/{categoryId}");

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
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            try
            {
                var response = await client.DeleteAsync($"{apiUrl}/DeleteProduct/{id}");

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
