using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Controllers
{
    public class AppointmentController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5297/api");
        private readonly HttpClient _httpClient;

        public AppointmentController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }

        [HttpGet]
        public async Task<IActionResult> ListAppointment()
        {
            List<AppointmentViewModel> appointmentsList = new List<AppointmentViewModel>();

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/Appointment/GetAllAppointments");

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    appointmentsList = JsonConvert.DeserializeObject<List<AppointmentViewModel>>(data);
                }
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Error: {ex.Message}");
            }

            return View(appointmentsList);
        }

        [HttpGet]
        public async Task<IActionResult> DetailAppointment(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Appointment ID");
            }

            AppointmentViewModel appointment = null;

            HttpResponseMessage response = await _httpClient.GetAsync($"/Appointment/GetByAppointmentId/{id}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                appointment = JsonConvert.DeserializeObject<AppointmentViewModel>(data);
            }

            if (appointment == null)
            {
                return NotFound("Appointment not found");
            }

            return View(appointment);
        }

        [HttpGet]
        public async Task<IActionResult> CreateAppointment()
        {
            await LoadDropdownDataAsync();
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment(AppointmentViewModel appointment)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(appointment);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync("/Appointment/Create", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ListAppointment");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi tạo mới cuộc hẹn.");
                }
            }

            await LoadDropdownDataAsync();
            return View(appointment);
        }

        [HttpGet]
        public async Task<IActionResult> EditAppointment(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Appointment ID");
            }

            AppointmentViewModel appointment = null;

            HttpResponseMessage response = await _httpClient.GetAsync($"/Appointment/GetByAppointmentId/{id}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                appointment = JsonConvert.DeserializeObject<AppointmentViewModel>(data);
            }

            if (appointment == null)
            {
                return NotFound("Appointment not found");
            }

            await LoadDropdownDataAsync();
            return View(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> EditAppointment(AppointmentViewModel appointment)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(appointment);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PutAsync($"/Appointment/UpdateAppointment/{appointment.Id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ListAppointment");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật cuộc hẹn.");
                }
            }

            await LoadDropdownDataAsync();
            return View(appointment);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteAppointment(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid Appointment ID");
            }

            HttpResponseMessage response = await _httpClient.DeleteAsync($"/Appointment/DeleteAppointment/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListAppointment");
            }
            else
            {
                return BadRequest("Có lỗi xảy ra khi xóa cuộc hẹn.");
            }
        }

        private async Task LoadDropdownDataAsync()
        {
            ViewBag.Customers = await GetSelectListAsync("/User/GetCustomers", "Id", "FullName");
            ViewBag.Employees = await GetSelectListAsync("/User/GetEmployees", "Id", "FullName");
            ViewBag.Services = await GetSelectListAsync("/Service/GetAllServices", "Id", "ServiceName");
        }

        private async Task<SelectList> GetSelectListAsync(string apiUrl, string valueField, string textField)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
            var items = new List<SelectListItem>();

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<dynamic>>(data);
                items = result.Select(x => new SelectListItem
                {
                    Value = x[valueField].ToString(),
                    Text = x[textField].ToString()
                }).ToList();
            }

            return new SelectList(items, "Value", "Text");
        }
    }
}
