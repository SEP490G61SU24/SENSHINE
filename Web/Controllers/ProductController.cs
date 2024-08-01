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
            List<CategoryViewModel> categoryList = new List<CategoryViewModel>();
            HttpResponseMessage response = await _httpClient.GetAsync("/api/ListAllProduct");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                viewList = JsonConvert.DeserializeObject<List<ProductViewModel>>(data);
            }
            HttpResponseMessage categoryResponse = await _httpClient.GetAsync("/api/ListAllCategory");
            if (categoryResponse.IsSuccessStatusCode)
            {
                string categoryData = await categoryResponse.Content.ReadAsStringAsync();
                categoryList = JsonConvert.DeserializeObject<List<CategoryViewModel>>(categoryData);
            }

            // Store categories in ViewBag
            ViewBag.Categories = categoryList;


            ViewData["Title"] = "List Products";
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
                var response = await _httpClient.DeleteAsync($"/api/Deleteproduct/{id}");

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
