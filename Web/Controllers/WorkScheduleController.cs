using API.Dtos;
using API.Models;
using API.Ultils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using System.Text.Json;
using Web.Models;
using Web.Utils;

namespace Web.Controllers
{
    public class WorkScheduleController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<WorkScheduleController> _logger;

        public WorkScheduleController(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<WorkScheduleController> logger)
             : base(configuration, clientFactory, logger)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10, string searchTerm = null)
        {
            try
            {
                var spaId = ViewData["SpaId"];
                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();

                var url = $"{apiUrl}/work-schedules?pageIndex={pageIndex}&pageSize={pageSize}&searchTerm={searchTerm}&spaId={spaId}";

                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var paginatedResult = await response.Content.ReadFromJsonAsync<PaginatedList<UserSlot>>();
                    paginatedResult.SearchTerm = searchTerm;
                    return View(paginatedResult);
                }
                else
                {
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }

        public async Task<IActionResult> My(int? selectedWeek = null, int? selectedYear = null)
        {
            try
            {
                UserDTO userProfile = ViewData["UserProfile"] as UserDTO;
                if (userProfile == null)
                {
                    return RedirectToAction("Login", "Auth");
                }

                var employeeId = userProfile.Id;
                ViewData["employeeId"] = employeeId;

                var apiUrl = _configuration["ApiUrl"];
                using var client = _clientFactory.CreateClient();

                // Nếu không có tuần nào được chọn, chọn tuần, năm hiện tại
                var currentWeek = selectedWeek ?? DateUtils.GetCurrentWeekOfYear();
                var currentYear = selectedYear ?? DateUtils.GetCurrentYear();

                //// Lấy danh sách năm
                //var yearsResponse = await client.GetAsync($"{apiUrl}/work-schedules/years?employeeId={employeeId}");
                //if (!yearsResponse.IsSuccessStatusCode)
                //{
                //	return View("Error");
                //}
                //var years = await yearsResponse.Content.ReadFromJsonAsync<IEnumerable<int>>();

                //// Lấy danh sách tuần
                //var weeksResponse = await client.GetAsync($"{apiUrl}/work-schedules/weeks?year={currentYear}&employeeId={employeeId}");
                //if (!weeksResponse.IsSuccessStatusCode)
                //{
                //	return View("Error");
                //}
                //var weeks = await weeksResponse.Content.ReadFromJsonAsync<IEnumerable<WeekOptionDTO>>();

                // Lấy lịch làm việc
                var workScheduleResponse = await client.GetAsync($"{apiUrl}/work-schedules/current-user/?employeeId={employeeId}&weekNumber={currentWeek}&year={currentYear}");
                if (!workScheduleResponse.IsSuccessStatusCode)
                {
                    return View("Error");
                }
                var workSchedules = await workScheduleResponse.Content.ReadFromJsonAsync<IEnumerable<WorkScheduleDTO>>();

                var viewData = new CurrentWorkScheduleViewModel
                {
                    //AvailableYears = years,
                    //AvailableWeeks = weeks,
                    SelectedYear = currentYear,
                    SelectedWeek = currentWeek,
                    WorkSchedules = workSchedules,
                };

                return View(viewData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }

        public async Task<IActionResult> Add()
        {
            try
            {
                // Ensure SpaId is properly cast from ViewData
                var spaId = ViewData["SpaId"] as string;
                if (string.IsNullOrEmpty(spaId))
                {
                    _logger.LogError("SpaId is null or empty.");
                    ViewData["Error"] = "SpaId is missing!";
                    return View("Error");
                }

                var apiUrl = _configuration["ApiUrl"];
                using var client = _clientFactory.CreateClient();

                // Fetch employees with SPA ID
                var employeeResponse = await client.GetAsync($"{apiUrl}/users/role/{(int)UserRoleEnum.STAFF}/?spaId={spaId}");

                if (!employeeResponse.IsSuccessStatusCode)
                {
                    _logger.LogError($"Failed to fetch employees for SpaId: {spaId}");
                    ViewData["Error"] = "Có lỗi xảy ra khi lấy danh sách nhân viên!";
                    return View("Error");
                }

                var employeeData = await employeeResponse.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>();
                ViewBag.Employees = employeeData;

                // Fetch slots
                var slots = await GetAllSlots();
                ViewBag.Slots = slots;

                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Add(int empId, List<int> slotIds, string date)
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];
                using var client = _clientFactory.CreateClient();
                var employeeResponse = await client.GetAsync($"{apiUrl}/users/role/" + (int)UserRoleEnum.STAFF);
                var employeeData = await employeeResponse.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>();

                // Validation logic
                if (slotIds == null || !slotIds.Any())
                {
                    return BadRequest("Please select at least one slot.");
                }

                DateTime parsedDate;
                if (!DateTime.TryParse(date, out parsedDate))
                {
                    return BadRequest("Invalid date format.");
                }

                // Iterate through each slotId and make the API call
                foreach (var slotId in slotIds)
                {
                    var content = new StringContent(JsonConvert.SerializeObject(new
                    {
                        employeeId = empId,
                        slotId,
                        date
                    }), Encoding.UTF8, "application/json");

                    var response = await client.PostAsync($"{apiUrl}/work-schedules/add-work-employee?employeeId={empId}&slotId={slotId}&date={date}", content);

                    // If any request fails, return an error message
                    if (!response.IsSuccessStatusCode)
                    {
                        ViewData["Error"] = "Có lỗi xảy ra khi thêm slot!";
                        return RedirectToAction("Add", "workschedule");
                    }
                }

                // If all requests succeeded, set the success message
                TempData["SuccessMsg"] = "Thêm thành công!";
                return RedirectToAction("Index", "workschedule");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }
        public class EditRequest
        {
            public int Id { get; set; }
        }

        [HttpPost]
        public async Task<IActionResult> Edit([FromBody] EditRequest request)
        {
            try
            {
                int id = request.Id;

                var apiUrl = _configuration["ApiUrl"];
                using var client = _clientFactory.CreateClient();

                var content = new StringContent(JsonConvert.SerializeObject(new
                {
                    id
                }), Encoding.UTF8, "application/json");

                var response = await client.PutAsync($"{apiUrl}/work-schedules/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true, message = "Cập nhật thành công!" });
                }
                else
                {
                    return Json(new { success = false, message = "Có lỗi xảy ra!" });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                return Json(new { success = false, message = "CÓ LỖI XẢY RA!" });
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
    }
}
