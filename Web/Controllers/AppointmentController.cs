﻿using API.Dtos;
using API.Models;
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
            List<ListAppointmentViewModel> appointmentsList = new List<ListAppointmentViewModel>();

            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/Appointment/GetAllAppointments");

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    appointmentsList = JsonConvert.DeserializeObject<List<ListAppointmentViewModel>>(data);
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

            ListAppointmentViewModel appointment = null;

            HttpResponseMessage response = await _httpClient.GetAsync($"/api/Appointment/GetByAppointmentId/{id}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                appointment = JsonConvert.DeserializeObject<ListAppointmentViewModel>(data);
            }

            if (appointment == null)
            {
                return NotFound("Appointment not found");
            }

            return View(appointment);
        }

        //xoa cuoc hen
        [HttpPost]
        public async Task<IActionResult> DeleteAppointment1(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Appointment ID không hợp lệ");
            }

            HttpResponseMessage response = await _httpClient.DeleteAsync(_httpClient.BaseAddress + $"/Appointment/DeleteAppointment/delete/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListAppointment");
            }
            else
            {
                return BadRequest("Có lỗi xảy ra khi xóa dịch vụ.");
            }
        }

        // tao cuoc hen moi
        [HttpGet]
        public async Task<IActionResult> CreateAppointment()
        {
            var services = await GetAvailableServices();
            var products = await GetAvailableProducts();
            var employees = await GetAvailableEmployees();
            var customers = await GetAvailableCustomers();

            ViewBag.Services = services ?? new List<ServiceViewModel>();
            ViewBag.Products = products ?? new List<ProductViewModel>();
            ViewBag.Employees = employees ?? new List<EmployeeViewModel>();
            ViewBag.Customers = customers ?? new List<UserDTO>();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment(AppointmentViewModel appointmentViewModel)
        {
            if (!ModelState.IsValid)
            {
                var services = await GetAvailableServices();
                var products = await GetAvailableProducts();
                var employees = await GetAvailableEmployees();
                var customers = await GetAvailableCustomers();

                ViewBag.Services = services;
                ViewBag.Products = products;
                ViewBag.Employees = employees;
                ViewBag.Customers = customers;

                return View(appointmentViewModel);
            }

            var appointmentDTO = new AppointmentDTO
            {
                CustomerId = appointmentViewModel.CustomerId,
                EmployeeId = appointmentViewModel.EmployeeId,
                AppointmentDate = appointmentViewModel.AppointmentDate,
                Status = appointmentViewModel.Status,
                Services = appointmentViewModel.SelectedServiceIds.Select(id => new ServiceDTO { Id = id, ServiceName = "" }).ToList(),
                Products = appointmentViewModel.SelectedProductIds.Select(id => new AppointmentDTO.AppointmentProductDTO { ProductId = id, ProductName = "" }).ToList()
            };

            string jsonString = JsonConvert.SerializeObject(appointmentDTO);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(_httpClient.BaseAddress + $"/Appointment/Create", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListAppointment");
            }

            string errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, errorMessage);

            var servicesList = await GetAvailableServices();
            var productsList = await GetAvailableProducts();
            var employeesList = await GetAvailableEmployees();
            var customersList = await GetAvailableCustomers();

            ViewBag.Services = servicesList;
            ViewBag.Products = productsList;
            ViewBag.Employees = employeesList;
            ViewBag.Customers = customersList;

            return View(appointmentViewModel);
        }

        //Chinh sua cuoc hen
        [HttpGet]
        public async Task<IActionResult> EditAppointment(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + $"/Appointment/GetByAppointmentId/{id}");

            if (!response.IsSuccessStatusCode)
            {
                return NotFound();
            }

            var appointmentDTO = JsonConvert.DeserializeObject<AppointmentDTO>(await response.Content.ReadAsStringAsync());
            var appointmentViewModel = new AppointmentViewModel
            {
                Id = appointmentDTO.Id,
                CustomerId = appointmentDTO.CustomerId,
                EmployeeId = appointmentDTO.EmployeeId,
                AppointmentDate = appointmentDTO.AppointmentDate ?? DateTime.Now,
                Status = appointmentDTO.Status,
                SelectedServiceIds = appointmentDTO.Services.Select(s => s.Id).ToList(),
                SelectedProductIds = appointmentDTO.Products.Select(p => p.ProductId).ToList()
            };

            var services = await GetAvailableServices();
            var products = await GetAvailableProducts();
            var employees = await GetAvailableEmployees();
            var customers = await GetAvailableCustomers();

            ViewBag.Services = services ?? new List<ServiceViewModel>();
            ViewBag.Products = products ?? new List<ProductViewModel>();
            ViewBag.Employees = employees ?? new List<EmployeeViewModel>();
            ViewBag.Customers = customers ?? new List<UserDTO>();

            return View(appointmentViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditAppointment(int id, AppointmentViewModel appointmentViewModel)
        {
            if (!ModelState.IsValid)
            {
                var services = await GetAvailableServices();
                var products = await GetAvailableProducts();
                var employees = await GetAvailableEmployees();
                var customers = await GetAvailableCustomers();

                ViewBag.Services = services;
                ViewBag.Products = products;
                ViewBag.Employees = employees;
                ViewBag.Customers = customers;

                return View(appointmentViewModel);
            }

            var appointmentDTO = new AppointmentDTO
            {
                Id = id,
                CustomerId = appointmentViewModel.CustomerId,
                EmployeeId = appointmentViewModel.EmployeeId,
                AppointmentDate = appointmentViewModel.AppointmentDate,
                Status = appointmentViewModel.Status,
                Services = appointmentViewModel.SelectedServiceIds.Select(id => new ServiceDTO { Id = id, ServiceName = "" }).ToList(),
                Products = appointmentViewModel.SelectedProductIds.Select(id => new AppointmentDTO.AppointmentProductDTO { ProductId = id, ProductName = "" }).ToList()
            };

            string jsonString = JsonConvert.SerializeObject(appointmentDTO);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync($"{_httpClient.BaseAddress}/Appointment/UpdateAppointment/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListAppointment");
            }

            string errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, errorMessage);

            var servicesList = await GetAvailableServices();
            var productsList = await GetAvailableProducts();
            var employeesList = await GetAvailableEmployees();
            var customersList = await GetAvailableCustomers();

            ViewBag.Services = servicesList;
            ViewBag.Products = productsList;
            ViewBag.Employees = employeesList;
            ViewBag.Customers = customersList;

            return View(appointmentViewModel);
        }


        //Lay ra danh sach cac du lieu lien quan 
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
        private async Task<List<ProductViewModel>> GetAvailableProducts()
        {
            List<ProductViewModel> products = new List<ProductViewModel>();
            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/ListAllProduct");

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                products = JsonConvert.DeserializeObject<List<ProductViewModel>>(jsonString);
            }

            return products;
        }

        private async Task<List<EmployeeViewModel>> GetAvailableEmployees()
        {
            List<EmployeeViewModel> employees = new List<EmployeeViewModel>();
            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/user/byRole/4");

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                employees = JsonConvert.DeserializeObject<List<EmployeeViewModel>>(jsonString);
            }

            return employees;
        }

        private async Task<List<UserDTO>> GetAvailableCustomers()
        {
            List<UserDTO> customers = new List<UserDTO>();
            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/user/byRole/5");

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                customers = JsonConvert.DeserializeObject<List<UserDTO>>(jsonString);
            }

            return customers;
        }

    }
}
