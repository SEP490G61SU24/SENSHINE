﻿using API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Web.Models;

namespace Web.Controllers
{
    public class AppointmentController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<UserController> _logger;

        public AppointmentController(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<UserController> logger)
             : base(configuration, clientFactory, logger)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> ListAppointment()
        {
            var client = _clientFactory.CreateClient();
            var apiUrl = _configuration["ApiUrl"];
            List<ListAppointmentViewModel> appointmentsList = new List<ListAppointmentViewModel>();

            try
            {
                HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Appointment/GetAllAppointments");

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
            var client = _clientFactory.CreateClient();
            var apiUrl = _configuration["ApiUrl"];
            if (id <= 0)
            {
                return BadRequest("Invalid Appointment ID");
            }

            ListAppointmentViewModel appointment = null;

            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Appointment/GetByAppointmentId/{id}");

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
            var client = _clientFactory.CreateClient();
            var apiUrl = _configuration["ApiUrl"];
            if (id <= 0)
            {
                return BadRequest("Appointment ID không hợp lệ");
            }

            HttpResponseMessage response = await client.DeleteAsync($"{apiUrl}/Appointment/DeleteAppointment/delete/{id}");

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
            var employees = await GetAvailableEmployees();
            var customers = await GetAvailableCustomers();

            ViewBag.Services = services ?? new List<ServiceViewModel>();
            ViewBag.Employees = employees ?? new List<AppointmentEmployeeViewModel>();
            ViewBag.Customers = customers ?? new List<AppointmentCustomerViewModel>();

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment(AppointmentViewModel appointmentViewModel)
        {
            var client = _clientFactory.CreateClient();
            var apiUrl = _configuration["ApiUrl"];
            if (!ModelState.IsValid)
            {
                var services = await GetAvailableServices();
                var employees = await GetAvailableEmployees();
                var customers = await GetAvailableCustomers();

                ViewBag.Services = services;
                ViewBag.Employees = employees;
                ViewBag.Customers = customers;

                return View(appointmentViewModel);
            }

            var appointmentDTO = new AppointmentDTO
            {
                CustomerId = appointmentViewModel.CustomerId.Value,
                EmployeeId = appointmentViewModel.EmployeeId.Value,
                AppointmentDate = appointmentViewModel.AppointmentDate,
                AppointmentSlot = appointmentViewModel.AppointmentSlot,
                RoomName = appointmentViewModel.RoomName,
                BedId = appointmentViewModel.BedId,
                Status = appointmentViewModel.Status,
                Services = appointmentViewModel.SelectedServiceIds.Select(id => new ServiceDTO { Id = id, ServiceName = "" }).ToList(),
            };

            string jsonString = JsonConvert.SerializeObject(appointmentDTO);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync($"{apiUrl}/Appointment/Create", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMsg"] = "Thêm lịch hẹn thành công!";
                return RedirectToAction("ListAppointment");
            }

            string errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, errorMessage);

            var servicesList = await GetAvailableServices();
            var employeesList = await GetAvailableEmployees();
            var customersList = await GetAvailableCustomers();

            ViewBag.Services = servicesList;
            ViewBag.Employees = employeesList;
            ViewBag.Customers = customersList;

            return View(appointmentViewModel);
        }

        //Chinh sua cuoc hen
        [HttpGet]
        public async Task<IActionResult> EditAppointment(int id)
        {
            var client = _clientFactory.CreateClient();
            var apiUrl = _configuration["ApiUrl"];
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Appointment/GetByAppointmentId/{id}");

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
                AppointmentSlot = appointmentDTO.AppointmentSlot,
                RoomName = appointmentDTO.RoomName,
                BedId = appointmentDTO.BedId,
                Status = appointmentDTO.Status,
                SelectedServiceIds = appointmentDTO.Services.Select(s => s.Id).ToList(),
            };

            var services = await GetAvailableServices();
            var employees = await GetAvailableEmployees();
            var customers = await GetAvailableCustomers();

            ViewBag.Services = services ?? new List<ServiceViewModel>();
            ViewBag.Employees = employees ?? new List<AppointmentEmployeeViewModel>();
            ViewBag.Customers = customers ?? new List<AppointmentCustomerViewModel>();

            return View(appointmentViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EditAppointment(int id, AppointmentViewModel appointmentViewModel)
        {
            var client = _clientFactory.CreateClient();
            var apiUrl = _configuration["ApiUrl"];
            if (!ModelState.IsValid)
            {
                var services = await GetAvailableServices();
                var employees = await GetAvailableEmployees();
                var customers = await GetAvailableCustomers();

                ViewBag.Services = services;
                ViewBag.Employees = employees;
                ViewBag.Customers = customers;

                return View(appointmentViewModel);
            }

            var appointmentDTO = new AppointmentDTO
            {
                Id = id,
                CustomerId = appointmentViewModel.CustomerId.Value,
                EmployeeId = appointmentViewModel.EmployeeId.Value,
                AppointmentDate = appointmentViewModel.AppointmentDate,
                AppointmentSlot = appointmentViewModel.AppointmentSlot,
                RoomName = appointmentViewModel.RoomName,
                BedId = appointmentViewModel.BedId,
                Status = appointmentViewModel.Status,
                Services = appointmentViewModel.SelectedServiceIds.Select(id => new ServiceDTO { Id = id, ServiceName = "" }).ToList(),
            };

            string jsonString = JsonConvert.SerializeObject(appointmentDTO);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PutAsync($"{apiUrl}/Appointment/UpdateAppointment/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMsg"] = "Sửa lịch hẹn thành công!";
                return RedirectToAction("ListAppointment");
            }

            string errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, errorMessage);

            var servicesList = await GetAvailableServices();
            var employeesList = await GetAvailableEmployees();
            var customersList = await GetAvailableCustomers();

            ViewBag.Services = servicesList;
            ViewBag.Employees = employeesList;
            ViewBag.Customers = customersList;

            return View(appointmentViewModel);
        }


        //Lay ra danh sach cac du lieu lien quan 
        private async Task<List<ServiceViewModel>> GetAvailableServices()
        {
            var client = _clientFactory.CreateClient();
            var apiUrl = _configuration["ApiUrl"];
            List<ServiceViewModel> services = new List<ServiceViewModel>();
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Service/GetAllServices");

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                services = JsonConvert.DeserializeObject<List<ServiceViewModel>>(jsonString);
            }

            return services;
        }
        
        private async Task<List<AppointmentEmployeeViewModel>> GetAvailableEmployees()
        {
            int? spaId = ViewData["SpaId"] != null && ViewData["SpaId"].ToString() != "ALL"
            ? int.Parse(ViewData["SpaId"].ToString())
            : (int?)null;
            var client = _clientFactory.CreateClient();
            var apiUrl = _configuration["ApiUrl"];
            List<AppointmentEmployeeViewModel> employees = new List<AppointmentEmployeeViewModel>();
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/users/role/4");

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                employees = JsonConvert.DeserializeObject<List<AppointmentEmployeeViewModel>>(jsonString);
                employees = employees.Where(e=> e.SpaId == spaId).ToList();
            }

            return employees;
        }

        private async Task<List<AppointmentCustomerViewModel>> GetAvailableCustomers()
        {
            int? spaId = ViewData["SpaId"] != null && ViewData["SpaId"].ToString() != "ALL"
            ? int.Parse(ViewData["SpaId"].ToString())
            : (int?)null;
            var client = _clientFactory.CreateClient();
            var apiUrl = _configuration["ApiUrl"];
            List<AppointmentCustomerViewModel> customers = new List<AppointmentCustomerViewModel>();
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/users/role/5");

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                customers = JsonConvert.DeserializeObject<List<AppointmentCustomerViewModel>>(jsonString);
                customers = customers.Where(c => c.SpaId == spaId).ToList();
            }

            return customers;
        }

    }
}
