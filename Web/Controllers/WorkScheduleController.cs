using API.Dtos;
using API.Ultils;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.Text;
using System.Text.Json;
using Web.Models;

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
					var paginatedResult = await response.Content.ReadFromJsonAsync<PaginatedList<WorkScheduleDTO>>();
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
				var currentWeek = selectedWeek ?? GetCurrentWeekOfYear();
				var currentYear = selectedYear ?? GetCurrentYear();

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

		private int GetCurrentWeekOfYear()
		{
			return CultureInfo.InvariantCulture.Calendar.GetWeekOfYear(DateTime.UtcNow, CalendarWeekRule.FirstFourDayWeek, DayOfWeek.Monday);
		}
		private int GetCurrentYear()
		{
			return DateTime.UtcNow.Year;
		}

		public async Task<IActionResult> Add()
		{
			try
			{
				var apiUrl = _configuration["ApiUrl"];
				var client = _clientFactory.CreateClient();

				// Lấy tất cả emp
				var employeeResponse = await client.GetAsync($"{apiUrl}/users/role/4");
				if (!employeeResponse.IsSuccessStatusCode)
				{
					return View("Error");
				}
				var employeeData = await employeeResponse.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>();

				var viewData = new WorkScheduleViewModel
				{
					Employees = employeeData,
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

		[HttpPost]
		public async Task<IActionResult> Add(WorkScheduleViewModel model)
		{
			try
			{
				var apiUrl = _configuration["ApiUrl"];
				using var client = _clientFactory.CreateClient();
				var employeeResponse = await client.GetAsync($"{apiUrl}/users/role/4");
				var employeeData = await employeeResponse.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>();

				if (!ModelState.IsValid)
				{
					model.Employees = employeeData;

					ViewData["Error"] = "Dữ liệu đầu vào thiếu!";

					return View(model);
				}

                var selectedSlot = model.TimeSlots[model.SelectedSlot];
				var StartDateTime = model.WorkDate.Date.Add(selectedSlot.StartTime);
				var EndDateTime = model.WorkDate.Date.Add(selectedSlot.EndTime);

				if (StartDateTime >= EndDateTime)
				{
					model.Employees = employeeData;
					ViewData["Error"] = "Giờ kết thúc phải sau giờ bắt đầu.";
					return View(model);
				}

				WorkScheduleDTO data = new WorkScheduleDTO
				{
					EmployeeId = model.EmployeeId,
					StartDateTime = StartDateTime,
					EndDateTime = EndDateTime,
					Status = model.Status
				};

				var json = JsonSerializer.Serialize(data);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				var response = await client.PostAsync($"{apiUrl}/work-schedules", content);

				if (response.IsSuccessStatusCode)
				{
					TempData["SuccessMsg"] = "Thêm thành công!";
					return RedirectToAction("Index", "workschedule");
				}
				else
				{
					model.Employees = employeeData;

					ViewData["Error"] = "Có lỗi xảy ra!";
					return View(model);
				}
			}
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }

		[HttpGet]
		public async Task<IActionResult> Edit(int id)
		{
			try
			{
				var apiUrl = _configuration["ApiUrl"];
				var client = _clientFactory.CreateClient();

				// Lấy tất cả emp
				var employeeResponse = await client.GetAsync($"{apiUrl}/users/role/4");
				if (!employeeResponse.IsSuccessStatusCode)
				{
					return View("Error");
				}
				var employeeData = await employeeResponse.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>();

				// Lấy ws
				var wsResponse = await client.GetAsync($"{apiUrl}/work-schedules/{id}");
				if (!wsResponse.IsSuccessStatusCode)
				{
					return View("Error");
				}
				var wsData = await wsResponse.Content.ReadFromJsonAsync<WorkScheduleDTO>();

				var viewData = new WorkScheduleViewModel
				{
					Id = id,
					WorkDate = wsData.StartDateTime.Value,
					Employees = employeeData,
					WorkScheduleData = wsData,
				};

				string selectedSlotKey = null;
				foreach (var slot in viewData.TimeSlots)
				{
					if (wsData.StartDateTime.HasValue && wsData.EndDateTime.HasValue)
					{
						var startTime = wsData.StartDateTime.Value.TimeOfDay;
						var endTime = wsData.EndDateTime.Value.TimeOfDay;
						if (startTime >= slot.Value.StartTime && endTime <= slot.Value.EndTime)
						{
							selectedSlotKey = slot.Key;
							break;
						}
					}
				}

				viewData.SelectedSlot = selectedSlotKey;

				return View(viewData);
			}
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }

		[HttpPost]
		public async Task<IActionResult> Edit(WorkScheduleViewModel model)
		{
			try
			{
				var apiUrl = _configuration["ApiUrl"];
				using var client = _clientFactory.CreateClient();
				var employeeResponse = await client.GetAsync($"{apiUrl}/users/role/4");
				var employeeData = await employeeResponse.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>();

				if (!ModelState.IsValid)
				{
                    // Lấy ws
                    var wsResponse = await client.GetAsync($"{apiUrl}/work-schedules/{model.Id}");
                    if (!wsResponse.IsSuccessStatusCode)
                    {
                        return View("Error");
                    }
                    var wsData = await wsResponse.Content.ReadFromJsonAsync<WorkScheduleDTO>();

                    model.WorkScheduleData = wsData;
					model.Employees = employeeData;

					ViewData["Error"] = "Dữ liệu đầu vào thiếu!";

					return View(model);
				}

                var selectedSlot = model.TimeSlots[model.SelectedSlot];
				var StartDateTime = model.WorkDate.Date.Add(selectedSlot.StartTime);
				var EndDateTime = model.WorkDate.Date.Add(selectedSlot.EndTime);

				if (StartDateTime >= EndDateTime)
                {
                    model.Employees = employeeData;
                    ViewData["Error"] = "Giờ kết thúc phải sau giờ bắt đầu.";
                    return View(model);
                }

                WorkScheduleDTO data = new WorkScheduleDTO
				{
					Id = model.Id,
					EmployeeId = model.EmployeeId,
					StartDateTime = StartDateTime,
					EndDateTime = EndDateTime,
					Status = model.Status
				};

				var json = JsonSerializer.Serialize(data);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				var response = await client.PutAsync($"{apiUrl}/work-schedules/{model.Id}", content);

				if (response.IsSuccessStatusCode)
				{
                    TempData["SuccessMsg"] = "Cập nhật thành công!";
					return RedirectToAction("Index", "WorkSchedule");
				}
				else
				{
                    var wsResponse = await client.GetAsync($"{apiUrl}/work-schedules/{model.Id}");
                    if (!wsResponse.IsSuccessStatusCode)
                    {
                        return View("Error");
                    }
                    var wsData = await wsResponse.Content.ReadFromJsonAsync<WorkScheduleDTO>();

                    model.WorkScheduleData = wsData;
					model.Employees = employeeData;

					ViewData["Error"] = "Có lỗi xảy ra!";
					return View(model);
				}
			}
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }

		[HttpDelete]
		public async Task<IActionResult> Delete(string id)
		{
			try
			{
				var apiUrl = _configuration["ApiUrl"];
				using var client = _clientFactory.CreateClient();
				var response = await client.DeleteAsync($"{apiUrl}/work-schedules/{id}");

				if (response.IsSuccessStatusCode)
				{
                    TempData["SuccessMsg"] = "Xóa thành công!";
					return RedirectToAction("Index", "workschedule");
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
	}
}
