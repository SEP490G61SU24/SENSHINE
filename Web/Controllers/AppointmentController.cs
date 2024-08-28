using API.Dtos;
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

            // Kiểm tra AppointmentDate và gán giá trị nếu không null
            DateTime appointmentDate = appointmentViewModel.AppointmentDate ?? DateTime.Now;

            // Gán giá trị cho AppointmentDate dựa trên AppointmentSlot
            switch (appointmentViewModel.AppointmentSlot)
            {
                case "Slot1":
                    appointmentViewModel.AppointmentDate = appointmentDate.Date.Add(new TimeSpan(8, 30, 0));
                    break;
                case "Slot2":
                    appointmentViewModel.AppointmentDate = appointmentDate.Date.Add(new TimeSpan(10, 0, 0));
                    break;
                case "Slot3":
                    appointmentViewModel.AppointmentDate = appointmentDate.Date.Add(new TimeSpan(11, 30, 0));
                    break;
                case "Slot4":
                    appointmentViewModel.AppointmentDate = appointmentDate.Date.Add(new TimeSpan(13, 0, 0));
                    break;
                case "Slot5":
                    appointmentViewModel.AppointmentDate = appointmentDate.Date.Add(new TimeSpan(14, 30, 0));
                    break;
                case "Slot6":
                    appointmentViewModel.AppointmentDate = appointmentDate.Date.Add(new TimeSpan(16, 0, 0));
                    break;
                case "Slot7":
                    appointmentViewModel.AppointmentDate = appointmentDate.Date.Add(new TimeSpan(17, 30, 0));
                    break;
                case "Slot8":
                    appointmentViewModel.AppointmentDate = appointmentDate.Date.Add(new TimeSpan(19, 0, 0));
                    break;
                case "Slot9":
                    appointmentViewModel.AppointmentDate = appointmentDate.Date.Add(new TimeSpan(20, 30, 0));
                    break;
                default:
                    break;
            }

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
                AppointmentDate = appointmentViewModel.AppointmentDate.Value,
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


        // GET: Chỉnh sửa cuộc hẹn
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
            var appointmentDate = appointmentDTO.AppointmentDate ?? DateTime.Now;

            // Gán giá trị AppointmentDate dựa trên AppointmentSlot
            switch (appointmentDTO.AppointmentSlot)
            {
                case "Slot1":
                    appointmentDTO.AppointmentDate = appointmentDate.Date.Add(new TimeSpan(8, 30, 0));
                    break;
                case "Slot2":
                    appointmentDTO.AppointmentDate = appointmentDate.Date.Add(new TimeSpan(10, 0, 0));
                    break;
                case "Slot3":
                    appointmentDTO.AppointmentDate = appointmentDate.Date.Add(new TimeSpan(11, 30, 0));
                    break;
                case "Slot4":
                    appointmentDTO.AppointmentDate = appointmentDate.Date.Add(new TimeSpan(13, 0, 0));
                    break;
                case "Slot5":
                    appointmentDTO.AppointmentDate = appointmentDate.Date.Add(new TimeSpan(14, 30, 0));
                    break;
                case "Slot6":
                    appointmentDTO.AppointmentDate = appointmentDate.Date.Add(new TimeSpan(16, 0, 0));
                    break;
                case "Slot7":
                    appointmentDTO.AppointmentDate = appointmentDate.Date.Add(new TimeSpan(17, 30, 0));
                    break;
                case "Slot8":
                    appointmentDTO.AppointmentDate = appointmentDate.Date.Add(new TimeSpan(19, 0, 0));
                    break;
                case "Slot9":
                    appointmentDTO.AppointmentDate = appointmentDate.Date.Add(new TimeSpan(20, 30, 0));
                    break;
                default:
                    break;
            }

            var appointmentViewModel = new AppointmentViewModel
            {
                Id = appointmentDTO.Id,
                CustomerId = appointmentDTO.CustomerId,
                EmployeeId = appointmentDTO.EmployeeId,
                AppointmentDate = appointmentDTO.AppointmentDate,
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

        // POST: Chỉnh sửa cuộc hẹn
        [HttpPost]
        public async Task<IActionResult> EditAppointment(int id, AppointmentViewModel appointmentViewModel)
        {
            var client = _clientFactory.CreateClient();
            var apiUrl = _configuration["ApiUrl"];

            // Gán giá trị AppointmentDate dựa trên AppointmentSlot
            DateTime appointmentDate = appointmentViewModel.AppointmentDate ?? DateTime.Now;

            switch (appointmentViewModel.AppointmentSlot)
            {
                case "Slot1":
                    appointmentViewModel.AppointmentDate = appointmentDate.Date.Add(new TimeSpan(8, 30, 0));
                    break;
                case "Slot2":
                    appointmentViewModel.AppointmentDate = appointmentDate.Date.Add(new TimeSpan(10, 0, 0));
                    break;
                case "Slot3":
                    appointmentViewModel.AppointmentDate = appointmentDate.Date.Add(new TimeSpan(11, 30, 0));
                    break;
                case "Slot4":
                    appointmentViewModel.AppointmentDate = appointmentDate.Date.Add(new TimeSpan(13, 0, 0));
                    break;
                case "Slot5":
                    appointmentViewModel.AppointmentDate = appointmentDate.Date.Add(new TimeSpan(14, 30, 0));
                    break;
                case "Slot6":
                    appointmentViewModel.AppointmentDate = appointmentDate.Date.Add(new TimeSpan(16, 0, 0));
                    break;
                case "Slot7":
                    appointmentViewModel.AppointmentDate = appointmentDate.Date.Add(new TimeSpan(17, 30, 0));
                    break;
                case "Slot8":
                    appointmentViewModel.AppointmentDate = appointmentDate.Date.Add(new TimeSpan(19, 0, 0));
                    break;
                case "Slot9":
                    appointmentViewModel.AppointmentDate = appointmentDate.Date.Add(new TimeSpan(20, 30, 0));
                    break;
                default:
                    break;
            }

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
                AppointmentDate = appointmentViewModel.AppointmentDate.Value,
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
            var client = _clientFactory.CreateClient();
            var apiUrl = _configuration["ApiUrl"];
            List<AppointmentEmployeeViewModel> employees = new List<AppointmentEmployeeViewModel>();
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/users/role/4");

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                employees = JsonConvert.DeserializeObject<List<AppointmentEmployeeViewModel>>(jsonString);
            }

            return employees;
        }

        private async Task<List<AppointmentCustomerViewModel>> GetAvailableCustomers()
        {
            var client = _clientFactory.CreateClient();
            var apiUrl = _configuration["ApiUrl"];
            List<AppointmentCustomerViewModel> customers = new List<AppointmentCustomerViewModel>();
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/users/role/5");

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                customers = JsonConvert.DeserializeObject<List<AppointmentCustomerViewModel>>(jsonString);
            }

            return customers;
        }

    }
}
