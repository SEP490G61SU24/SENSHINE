using API.Dtos;
using API.Models;
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
        public async Task<IActionResult> ListAppointment(DateTime? date)
        {
            // Use DateTime.Now if no date is provided
            var selectedDate = date ?? DateTime.Now;

            var beds = await GetAllBedsInSpa();
            var slots = await GetAllSlots();
            ViewBag.Beds = beds;
            ViewBag.Slots = slots;
            ViewBag.ApiUrl = _configuration["ApiUrl"];
            ViewBag.Date = selectedDate.ToString("yyyy-MM-dd"); // Format the date for use in the API call
            return View();
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

            ViewBag.Services = services ?? new List<ServiceViewModel>();

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

                ViewBag.Services = services;

                return View(appointmentViewModel);
            }

            var appointmentDTO = new AppointmentDTO
            {
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


            ViewBag.Services = servicesList;


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



            var appointmentViewModel = new AppointmentViewModel
            {
                Id = appointmentDTO.Id,
                CustomerId = appointmentDTO.CustomerId,
                EmployeeId = appointmentDTO.EmployeeId,
                AppointmentDate = appointmentDTO.AppointmentDate,

                BedId = appointmentDTO.BedId,
                Status = appointmentDTO.Status,
                SelectedServiceIds = appointmentDTO.Services.Select(s => s.Id).ToList(),
            };

            var services = await GetAvailableServices();


            ViewBag.Services = services ?? new List<ServiceViewModel>();

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


                ViewBag.Services = services;


                return View(appointmentViewModel);
            }

            var appointmentDTO = new AppointmentDTO
            {
                Id = id,
                CustomerId = appointmentViewModel.CustomerId.Value,
                EmployeeId = appointmentViewModel.EmployeeId.Value,

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

            return View(appointmentViewModel);
        }

        private async Task<List<SlotDTO>> GetAllSlots()
        {
            var client = _clientFactory.CreateClient();
            var apiUrl = _configuration["ApiUrl"];
            List<SlotDTO> slots = new List<SlotDTO>();
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Appointment/GetAllSlots");

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                slots = JsonConvert.DeserializeObject<List<SlotDTO>>(jsonString);
            }

            return slots;
        }

        private async Task<List<BedDTO>> GetAllBedsInSpa()
        {
            int? spaId = ViewData["SpaId"] != null && ViewData["SpaId"].ToString() != "ALL"
                ? int.Parse(ViewData["SpaId"].ToString())
                : (int?)null;

            var client = _clientFactory.CreateClient();
            var apiUrl = _configuration["ApiUrl"];
            List<RoomDTO> rooms = new List<RoomDTO>();
            List<BedDTO> beds = new List<BedDTO>();

            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Room/GetBySpaId?spaId=" + spaId);

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                rooms = JsonConvert.DeserializeObject<List<RoomDTO>>(jsonString);

                foreach (var room in rooms)
                {
                    HttpResponseMessage responseBedsInThisRoom = await client.GetAsync($"{apiUrl}/Bed/GetByRoomId/ByRoomId/" + room.Id);
                    string jsonStringBedsInThisRoom = await responseBedsInThisRoom.Content.ReadAsStringAsync();
                    List<BedDTO> bedsInThisRoom = JsonConvert.DeserializeObject<List<BedDTO>>(jsonStringBedsInThisRoom);

                    beds.AddRange(bedsInThisRoom);
                }
            }

            return beds;
        }

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

        private async Task<List<ComboViewModel>> GetAvailableCombos()
        {
            var client = _clientFactory.CreateClient();
            var apiUrl = _configuration["ApiUrl"];
            List<ComboViewModel> combos = new List<ComboViewModel>();
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Combo/GetAllCombo");

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                combos = JsonConvert.DeserializeObject<List<ComboViewModel>>(jsonString);
            }

            return combos;
        }

        private async Task<List<UserDTO>> GetAvailableEmployeesInSlot(DateTime date, int slotId)
        {
            int? spaId = ViewData["SpaId"]?.ToString() != "ALL"
                ? int.Parse(ViewData["SpaId"].ToString())
                : (int?)null;

            var client = _clientFactory.CreateClient();
            var apiUrl = _configuration["ApiUrl"];

            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/users/role/4");

            if (!response.IsSuccessStatusCode) return new List<UserDTO>();

            string jsonString = await response.Content.ReadAsStringAsync();
            var employees = JsonConvert.DeserializeObject<List<UserDTO>>(jsonString)
                .Where(e => e.SpaId == spaId)
                .ToList();

            var availableEmployees = new List<UserDTO>();

            foreach (var employee in employees)
            {
                HttpResponseMessage response2 = await client.GetAsync($"{apiUrl}/Appointment/UserBooked?userId={employee.Id}&slotId={slotId}&date={date}");
                bool isBooked = JsonConvert.DeserializeObject<bool>(await response2.Content.ReadAsStringAsync());

                if (!isBooked)
                {
                    availableEmployees.Add(employee);
                }
            }

            return availableEmployees;
        }

        private async Task<List<UserDTO>> GetAvailableCustomersInSlot(DateTime date, int slotId)
        {
            int? spaId = ViewData["SpaId"]?.ToString() != "ALL"
                ? int.Parse(ViewData["SpaId"].ToString())
                : (int?)null;

            var client = _clientFactory.CreateClient();
            var apiUrl = _configuration["ApiUrl"];

            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/users/role/5");

            if (!response.IsSuccessStatusCode) return new List<UserDTO>();

            string jsonString = await response.Content.ReadAsStringAsync();
            var customers = JsonConvert.DeserializeObject<List<UserDTO>>(jsonString)
                .Where(e => e.SpaId == spaId)
                .ToList();

            var availableCustomers = new List<UserDTO>();

            foreach (var customer in customers)
            {
                HttpResponseMessage response2 = await client.GetAsync($"{apiUrl}/Appointment/UserBooked?userId={customer.Id}&slotId={slotId}&date={date}");
                bool isBooked = JsonConvert.DeserializeObject<bool>(await response2.Content.ReadAsStringAsync());

                if (!isBooked)
                {
                    availableCustomers.Add(customer);
                }
            }

            return availableCustomers;
        }
    }
}
