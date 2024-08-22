using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Web.Models;
using System.Collections.Generic;
using API.Dtos;
using API.Ultils;

namespace Web.Controllers
{
    public class ComboController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<UserController> _logger;


        public ComboController(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<UserController> logger) : base(configuration, clientFactory, logger)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
        }
    

        // Hiển thị danh sách combo
        public async Task<IActionResult> Index(int pageIndex = 1,int pageSize =10,string searchTerm=null)
        {

            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            
            List<ComboViewModel> comboList = new List<ComboViewModel>();

            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Combo/GetAllCombosPaging?pageIndex={pageIndex}&pageSize={pageSize}&searchTerm={searchTerm}");

            if (response.IsSuccessStatusCode)
            {
                var paginatedResult = await response.Content.ReadFromJsonAsync<PaginatedList<ComboViewModel>>();
                paginatedResult.SearchTerm=searchTerm;
                return View(paginatedResult);
            }
            return View("Error");
        }

        // Tạo combo mới
        [HttpGet]
        public async Task<IActionResult> CreateCombo()
        {
            var services = await GetAvailableServices();
            ViewBag.Services = services;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCombo(ComboViewModel comboViewModel)
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();

            if (!ModelState.IsValid)
            {
                var services = await GetAvailableServices();
                ViewBag.Services = services;
                return View(comboViewModel);
            }

            var comboDTO = new ComboDTO
            {
                Name = comboViewModel.Name,
                Quantity = comboViewModel.Quantity,
                Note = comboViewModel.Note,
                Price = comboViewModel.Price,
                Discount = comboViewModel.Discount,
                SalePrice = comboViewModel.SalePrice,
                Services = comboViewModel.SelectedServiceIds.Select(id => new ServiceDTO { Id = id,ServiceName="" }).ToList()
            };

            string jsonString = JsonConvert.SerializeObject(comboDTO);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync($"{apiUrl}/Combo/Create", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index"); // Chuyển hướng về trang danh sách combo
            }

            string errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, errorMessage);
            var servicesList = await GetAvailableServices();
            ViewBag.Services = servicesList;
            return View(comboViewModel);
        }

        private async Task<List<ServiceViewModel>> GetAvailableServices()
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            List<ServiceViewModel> services = new List<ServiceViewModel>();

            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Service/GetAllServices");

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                services = JsonConvert.DeserializeObject<List<ServiceViewModel>>(jsonString);
            }

            return services;
        }
        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {

            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            try
            {
                var response = await client.DeleteAsync($"{apiUrl}/Combo/DeleteCombo/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true });
                }

                return Json(new { success = false, message = "An error occurred while deleting the combo." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An unexpected error occurred." });
            }
        }
        // GET: /Combo/EditCombo/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var combo = await GetComboById(id);
            if (combo == null)
            {
                return NotFound();
            }

            var services = await GetAvailableServices();
            ViewBag.Services = services;

            return View(combo);
        }

        // POST: /Combo/EditCombo
        [HttpPost]
        public async Task<IActionResult> Edit(ComboViewModel comboViewModel)
        {

            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            if (!ModelState.IsValid)
            {
                var services = await GetAvailableServices();
                ViewBag.Services = services;
                return View(comboViewModel);
            }

            var comboDTO = new ComboDTO
            {
                Id = comboViewModel.Id,
                Name = comboViewModel.Name,
                Quantity = comboViewModel.Quantity,
                Note = comboViewModel.Note,
                Price = comboViewModel.Price,
                Discount = comboViewModel.Discount,
                SalePrice = comboViewModel.SalePrice,
                Services = comboViewModel.SelectedServiceIds.Select(id => new ServiceDTO { Id = id, ServiceName = "" }).ToList()
            };

            string jsonString = JsonConvert.SerializeObject(comboDTO);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PutAsync($"{apiUrl}/Combo/UpdateCombo/{comboViewModel.Id}", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            string errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, errorMessage);
            var servicesList = await GetAvailableServices();
            ViewBag.Services = servicesList;
            return View(comboViewModel);
        }

        private async Task<ComboViewModel> GetComboById(int id)
        {


            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Combo/GetByID?IdCombo={id}");

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ComboViewModel>(jsonString);
            }

            return null;
        }


    }
}
