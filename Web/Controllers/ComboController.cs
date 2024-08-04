using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Web.Models;
using System.Collections.Generic;
using API.Dtos;

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

            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/Combo/GetAllCombo");

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                comboList = JsonConvert.DeserializeObject<List<ComboViewModel>>(jsonString);
            }
            return View(comboList);
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

            HttpResponseMessage response = await _httpClient.PostAsync(_httpClient.BaseAddress +$"/Combo/Create", content);

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
            List<ServiceViewModel> services = new List<ServiceViewModel>();

            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/Service/GetAllServices");

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
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/Combo/DeleteCombo/{id}");

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

            HttpResponseMessage response = await _httpClient.PutAsync(_httpClient.BaseAddress + $"/Combo/UpdateCombo/{comboViewModel.Id}", content);

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
            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + $"/Combo/GetByID?IdCombo={id}");

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ComboViewModel>(jsonString);
            }

            return null;
        }


    }
}
