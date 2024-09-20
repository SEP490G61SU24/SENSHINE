using API.Dtos;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Differencing;
using Newtonsoft.Json;
using System;
using System.Text;
using Web.Models;
using static System.Reflection.Metadata.BlobBuilder;

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
        [HttpGet]
        public async Task<IActionResult> ListAppointment(DateTime? date)
        {
            int? spaId = ViewData["SpaId"]?.ToString() != "ALL"
                ? int.Parse(ViewData["SpaId"].ToString())
                : (int?)null;

            if (TempData["SelectedDate"] != null)
            {
                var tempDate = (DateTime)TempData["SelectedDate"];
                ViewBag.Date = tempDate.ToString("yyyy-MM-dd");
            }
            else
            {
                // Use DateTime.Now if no date is provided
                var selectedDate = date ?? DateTime.Now;
                ViewBag.Date = selectedDate.ToString("yyyy-MM-dd");
            }
            var beds = await GetAllBedsInSpa();
            var slots = await GetAllSlots();
            ViewBag.Beds = beds;
            ViewBag.Slots = slots;
            ViewBag.ApiUrl = _configuration["ApiUrl"];
            ViewBag.SpaId = spaId;
            return View();
        }

        public async Task<PartialViewResult> CreateAppointmentContent(int bedId, int slotId, string date)
        {
            var client = _clientFactory.CreateClient();
            var apiUrl = _configuration["ApiUrl"];
            string dateBook = null;
            ViewBag.BedId = bedId;
            ViewBag.SlotId = slotId;
            var beds = await GetAllBedsInSpa();
            var slots = await GetAllSlots();
            var thisBed = beds.FirstOrDefault(b => b.Id == bedId);
            var thisSlot = slots.FirstOrDefault(s => s.Id == slotId);
            if (thisBed != null && thisSlot != null)
            {
                ViewBag.BedRoomName = thisBed.BedNumber + " " + thisBed.RoomName;
                ViewBag.SlotName = thisSlot.SlotName + " (" + thisSlot.TimeFrom + " - " + thisSlot.TimeTo + ")";
            }
            ViewBag.Date = date;
            var services = await GetAvailableServices();
            var combos = await GetAvailableCombos();
            ViewBag.Services = services;
            ViewBag.Combos = combos;

            var customers = await GetAvailableCustomersInSlot(DateTime.Parse(date), slotId);
            foreach (var customer in customers)
            {
                customer.FullName = string.Join(", ", customer.FullName ?? "", customer.Phone ?? "").Trim();
            }
            var employees = await GetAvailableEmployeesInSlot(DateTime.Parse(date), slotId);

            // Check if no customers or employees are available
            if (!customers.Any())
            {
                ViewData["Error"] = "Không còn khách hàng trống slot này.";
            }

            if (!employees.Any())
            {
                ViewData["Error"] = "Không còn nhân viên trống slot này.";
            }

            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Appointment/GetSlotById?id=" + slotId);

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                var slot = JsonConvert.DeserializeObject<SlotDTO>(jsonString);
                dateBook = date + " " + slot.TimeFrom.ToString();
            }

            if (DateTime.ParseExact(dateBook, "yyyy-MM-dd HH:mm:ss", null) < DateTime.Now)
            {
                ViewData["Error"] = "Không thể book lịch trong quá khứ.";
            }

            // Prepare customers and employees data for selection
            ViewBag.Customers = new SelectList(customers, "Id", "FullName");
            ViewBag.Employees = new SelectList(employees, "Id", "FullName");

            return PartialView("_CreateAppointmentContent");
        }

        [HttpPost]
        public async Task<IActionResult> CreateAppointment(AppointmentDTO model, string selectedServiceIds, string selectedComboIds,decimal totalPrice)
        {
            var client = _clientFactory.CreateClient();
            var apiUrl = _configuration["ApiUrl"];

            if (!ModelState.IsValid)
            {
                return RedirectToAction("ListAppointment");
            }
            var use = await LoadUserAsync();
            string jsonString = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");
            List<int> selectedIds2 = JsonConvert.DeserializeObject<List<int>>(selectedServiceIds);
            List<int> selectedIds3 = JsonConvert.DeserializeObject<List<int>>(selectedComboIds);
            var invoice = new InvoiceViewModel
            {
                ServiceQuantities = selectedIds2
                            .GroupBy(id => id)
                            .ToDictionary(group => group.Key, group => (int?)group.Count()),
                ComboQuantities = selectedIds3
                            .GroupBy(id => id)
                            .ToDictionary(group => group.Key, group => (int?)group.Count()),
                CustomerId = model.CustomerId,
                ComboIds = JsonConvert.DeserializeObject<List<int>>(selectedComboIds).Distinct().ToList(),
                ServiceIds = JsonConvert.DeserializeObject<List<int>>(selectedServiceIds).Distinct().ToList(),
                SpaId = use.SpaId,
                InvoiceDate = DateTime.Now,
                Status = "Pending",
                Amount = totalPrice,
                Description = "Hóa đơn đặt lịch của anh/chị "+ model.CustomerId
            };

            var contentInvoice = new StringContent(JsonConvert.SerializeObject(invoice), Encoding.UTF8, "application/json");
            HttpResponseMessage responseInvoice = await client.PostAsync($"{apiUrl}/AddInvoice", contentInvoice);

            if (!responseInvoice.IsSuccessStatusCode)
            {
                string errorMessage = await responseInvoice.Content.ReadAsStringAsync();
                _logger.LogError("Failed to create invoice: {0}", errorMessage);
                return Json(new { success = false, error = $"An error occurred while adding the invoice: {errorMessage}" });
            }


            var content1 = new StringContent(JsonConvert.SerializeObject(new
            {
                userId = model.CustomerId,
                slotId = model.SlotId,
                date = model.AppointmentDate
            }), Encoding.UTF8, "application/json");

            var content2 = new StringContent(JsonConvert.SerializeObject(new
            {
                userId = model.EmployeeId,
                slotId = model.SlotId,
                date = model.AppointmentDate
            }), Encoding.UTF8, "application/json");

            var content3 = new StringContent(JsonConvert.SerializeObject(new
            {
                bedId = model.BedId,
                slotId = model.SlotId,
                date = model.AppointmentDate
            }), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PostAsync($"{apiUrl}/Appointment/Create", content);

            if (response.IsSuccessStatusCode)
            {
                HttpResponseMessage response1 = await client.PostAsync($"{apiUrl}/Appointment/BookUser?userId={model.CustomerId}&slotId={model.SlotId}&date={model.AppointmentDate}", content1);
                HttpResponseMessage response2 = await client.PostAsync($"{apiUrl}/Appointment/BookUser?userId={model.EmployeeId}&slotId={model.SlotId}&date={model.AppointmentDate}", content2);
                HttpResponseMessage response3 = await client.PostAsync($"{apiUrl}/Appointment/BookBed?bedId={model.BedId}&slotId={model.SlotId}&date={model.AppointmentDate}", content3);

                if (response1.IsSuccessStatusCode && response2.IsSuccessStatusCode && response3.IsSuccessStatusCode)
                {
                    TempData["SuccessMsg"] = "Tạo lịch hẹn thành công!";
                    TempData["SelectedDate"] = model.AppointmentDate;
                    return RedirectToAction("ListAppointment");
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to book a slot: {0}", errorMessage);
                    ViewData["Error"] = $"An error occurred while booking a slot: {errorMessage}";
                    return View("ErrorView");  // You can direct it to an error view or the same view with error handling
                }
            }
            else
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to create appointment: {0}", errorMessage);
                ViewData["Error"] = $"An error occurred while creating a new appointment: {errorMessage}";
                return View();  // Return the same view with error
            }
        }

        public async Task<PartialViewResult> UpdateAppointmentContent(int bedId, int slotId, string date)
        {
            var client = _clientFactory.CreateClient();
            var apiUrl = _configuration["ApiUrl"];
            var model = new AppointmentDTO();

            // Fetch the appointment details by bed, slot, and date
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Appointment/GetByBedSlotDate?bedId={bedId}&slotId={slotId}&date={date}");
            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                model = JsonConvert.DeserializeObject<AppointmentDTO>(jsonString);

                // Ensure that ComboIDs and ServiceIDs are populated
                model.ComboIDs = model.ComboIDs ?? new List<int>();
                model.ServiceIDs = model.ServiceIDs ?? new List<int>();
            }

            // Prepare the view with the relevant data
            ViewBag.BedId = model.BedId;
            ViewBag.SlotId = model.SlotId;
            ViewBag.Date = model.AppointmentDate.ToString("yyyy-MM-dd");
            var beds = await GetAllBedsInSpa();
            var slots = await GetAllSlots();
            var thisBed = beds.FirstOrDefault(b => b.Id == bedId);
            var thisSlot = slots.FirstOrDefault(s => s.Id == slotId);
            if (thisBed != null && thisSlot != null)
            {
                ViewBag.BedRoomName = thisBed.BedNumber + " " + thisBed.RoomName;
                ViewBag.SlotName = thisSlot.SlotName + " (" + thisSlot.TimeFrom + " - " + thisSlot.TimeTo + ")";
            }
            // Fetch services and combos
            var services = await GetAvailableServices();
            var combos = await GetAvailableCombos();
            ViewBag.Services = services;
            ViewBag.Combos = combos;

            // Fetch customers in this slot and the selected customer
            var customers = await GetAvailableCustomersInSlot(model.AppointmentDate, model.SlotId);
            var responseCus = await client.GetAsync($"{apiUrl}/users/{model.CustomerId}");
            if (responseCus.IsSuccessStatusCode)
            {
                var cus = await responseCus.Content.ReadFromJsonAsync<UserDTO>();
                ViewBag.OldCusId = cus.Id;
                customers.Add(cus);
            }
            else
            {
                ViewData["Error"] = "Error fetching customer data";
            }

            // Clean up customer display name
            foreach (var customer in customers)
            {
                customer.FullName = string.Join(", ", customer.FullName ?? "", customer.Phone ?? "").Trim();
            }

            // Fetch employees in this slot and the selected employee
            var employees = await GetAvailableEmployeesInSlot(model.AppointmentDate, model.SlotId);
            var responseEmp = await client.GetAsync($"{apiUrl}/users/{model.EmployeeId}");
            if (responseEmp.IsSuccessStatusCode)
            {
                var emp = await responseEmp.Content.ReadFromJsonAsync<UserDTO>();
                ViewBag.OldEmpId = emp.Id;
                employees.Add(emp);
            }
            else
            {
                ViewData["Error"] = "Error fetching employee data";
            }

            // Get slot details and calculate the date + time
            HttpResponseMessage response2 = await client.GetAsync($"{apiUrl}/Appointment/GetSlotById?id=" + model.SlotId);
            string dateBook = null;
            if (response2.IsSuccessStatusCode)
            {
                string jsonString2 = await response2.Content.ReadAsStringAsync();
                var slot = JsonConvert.DeserializeObject<SlotDTO>(jsonString2);
                dateBook = model.AppointmentDate.ToString("yyyy-MM-dd") + " " + slot.TimeFrom.ToString();
            }

            // Prevent updating past appointments
            if (DateTime.ParseExact(dateBook, "yyyy-MM-dd HH:mm:ss", null) < DateTime.Now)
            {
                ViewData["Error"] = "Không thể cập nhật hoặc xóa lịch trong quá khứ.";
            }

            // Prepare the select lists for customers and employees
            ViewBag.Customers = new SelectList(customers, "Id", "FullName");
            ViewBag.Employees = new SelectList(employees, "Id", "FullName");

            return PartialView("_UpdateAppointmentContent", model);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateAppointment(AppointmentDTO model, string oldCusId, string oldEmpId)
        {
            var client = _clientFactory.CreateClient();
            var apiUrl = _configuration["ApiUrl"];

            if (!ModelState.IsValid)
            {
                return RedirectToAction("ListAppointment");
            }

            string jsonString = JsonConvert.SerializeObject(model);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            var content1 = new StringContent(JsonConvert.SerializeObject(new
            {
                userId = oldCusId,
                slotId = model.SlotId,
                date = model.AppointmentDate
            }), Encoding.UTF8, "application/json");

            var content2 = new StringContent(JsonConvert.SerializeObject(new
            {
                userId = oldEmpId,
                slotId = model.SlotId,
                date = model.AppointmentDate
            }), Encoding.UTF8, "application/json");

            var content3 = new StringContent(JsonConvert.SerializeObject(new
            {
                userId = model.CustomerId,
                slotId = model.SlotId,
                date = model.AppointmentDate
            }), Encoding.UTF8, "application/json");

            var content4 = new StringContent(JsonConvert.SerializeObject(new
            {
                userId = model.EmployeeId,
                slotId = model.SlotId,
                date = model.AppointmentDate
            }), Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PutAsync($"{apiUrl}/Appointment/UpdateAppointment", content);

            if (response.IsSuccessStatusCode)
            {
                HttpResponseMessage response1 = await client.PostAsync($"{apiUrl}/Appointment/BookUser?userId={oldCusId}&slotId={model.SlotId}&date={model.AppointmentDate}", content1);
                HttpResponseMessage response2 = await client.PostAsync($"{apiUrl}/Appointment/BookUser?userId={oldEmpId}&slotId={model.SlotId}&date={model.AppointmentDate}", content2);
                HttpResponseMessage response3 = await client.PostAsync($"{apiUrl}/Appointment/BookUser?userId={model.CustomerId}&slotId={model.SlotId}&date={model.AppointmentDate}", content3);
                HttpResponseMessage response4 = await client.PostAsync($"{apiUrl}/Appointment/BookUser?userId={model.EmployeeId}&slotId={model.SlotId}&date={model.AppointmentDate}", content4);

                if (response1.IsSuccessStatusCode && response2.IsSuccessStatusCode && response3.IsSuccessStatusCode && response4.IsSuccessStatusCode)
                {
                    TempData["SuccessMsg"] = "Cập nhật lịch hẹn thành công!";
                    TempData["SelectedDate"] = model.AppointmentDate;
                    return RedirectToAction("ListAppointment");
                }
                else
                {
                    string errorMessage = await response.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to update an appointment: {0}", errorMessage);
                    ViewData["Error"] = $"An error occurred while update an appointment: {errorMessage}";
                    return View("ErrorView");  // You can direct it to an error view or the same view with error handling
                }
            }
            else
            {
                string errorMessage = await response.Content.ReadAsStringAsync();
                _logger.LogError("Failed to create appointment: {0}", errorMessage);
                ViewData["Error"] = $"An error occurred while update an appointment: {errorMessage}";
                return View();  // Return the same view with error
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(string id)
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];
                using var client = _clientFactory.CreateClient();
                var response = await client.DeleteAsync($"{apiUrl}/Appointment/DeleteAppointment?id={id}");

                if (response.IsSuccessStatusCode)
                {
                    return Ok(new { message = "Xóa thành công!" });  // Return 200 OK with a success message
                }
                else
                {
                    ViewData["Error"] = "Có lỗi xảy ra!";
                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
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
                    foreach (var bed in bedsInThisRoom)
                    {
                        bed.RoomName = room.RoomName;
                    }
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
                combos = combos.Where(c => c.Quantity == 1).ToList();
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
                HttpResponseMessage response2 = await client.GetAsync($"{apiUrl}/Appointment/UserAvailable?userId={employee.Id}&slotId={slotId}&date={date}");
                bool isAvailable = JsonConvert.DeserializeObject<bool>(await response2.Content.ReadAsStringAsync());

                if (isAvailable)
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
