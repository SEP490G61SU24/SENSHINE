using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using System.Text;
using System.Globalization;
using API.Dtos;
using API.Ultils;
using Web.Models;

namespace Web.Controllers
{
    public class UserController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<UserController> _logger;

        public UserController(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<UserController> logger)
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
                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();

                var url = $"{apiUrl}/users?pageIndex={pageIndex}&pageSize={pageSize}&searchTerm={searchTerm}";

                var response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    var paginatedResult = await response.Content.ReadFromJsonAsync<PaginatedList<UserDTO>>();
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
                _logger.LogError(ex, "Error during login");
                ViewData["Error"] = "An error occurred";
                return View();
            }
        }

        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(UserDTO user)
        {
            try
            {
                user.Password = "123456";

                string[] nameArr = user.FullName.Split(" ");

                if (nameArr.Length < 2)
                {
                    user.FirstName = null;
                    user.MidName = null;
                    user.LastName = nameArr[0];
                }
                else if (nameArr.Length < 3)
                {
                    user.FirstName = nameArr[0];
                    user.MidName = null;
                    user.LastName = nameArr[1];
                }
                else
                {
                    user.FirstName = nameArr[0];
                    user.LastName = nameArr[nameArr.Length - 1];
                    user.MidName = string.Join(" ", nameArr.Skip(1).Take(nameArr.Length - 2));
                }

                user.UserName = (RemoveDiacritics(user.LastName) + user.ProvinceCode + GenerateRandomString(4)).ToLower();

                var apiUrl = _configuration["ApiUrl"];

                var json = JsonSerializer.Serialize(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var client = _clientFactory.CreateClient();
                var response = await client.PostAsync($"{apiUrl}/users", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMsg"] = "Thêm người dùng thành công!";
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    ViewData["Error"] = responseString;
                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                ViewData["Error"] = "An error occurred";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();
                var response = await client.GetAsync($"{apiUrl}/users/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadFromJsonAsync<UserDTO>();
                    user.FullName = string.Join(" ", new[] { user.FirstName, user.MidName, user.LastName }.Where(name => !string.IsNullOrEmpty(name)));
                    return View(user);
                }
                else
                {
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                ViewData["Error"] = "An error occurred";
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserDTO user)
        {
            try
            {
                string[] nameArr = user.FullName.Split(" ");

                if (nameArr.Length < 2)
                {
                    user.FirstName = null;
                    user.MidName = null;
                    user.LastName = nameArr[0];
                }
                else if (nameArr.Length < 3)
                {
                    user.FirstName = nameArr[0];
                    user.MidName = null;
                    user.LastName = nameArr[1];
                }
                else
                {
                    user.FirstName = nameArr[0];
                    user.LastName = nameArr[nameArr.Length - 1];
                    user.MidName = string.Join(" ", nameArr.Skip(1).Take(nameArr.Length - 2));
                }

                var apiUrl = _configuration["ApiUrl"];

                var json = JsonSerializer.Serialize(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var client = _clientFactory.CreateClient();
                var response = await client.PutAsync($"{apiUrl}/users/{user.Id}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMsg"] = "Sửa người dùng thành công!";
                    return RedirectToAction("Index", "User");
                }
                else
                {
					var responseString = await response.Content.ReadAsStringAsync();
					ViewData["Error"] = responseString;
					return View(user);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                ViewData["Error"] = "An error occurred";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> ChangePass(int id)
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();
                var response = await client.GetAsync($"{apiUrl}/users/{id}");
                if (!response.IsSuccessStatusCode)
                {
					var responseString = await response.Content.ReadAsStringAsync();
					ViewData["Error"] = responseString;
					return View("Error");
				}

                var user = await response.Content.ReadFromJsonAsync<UserDTO>();

                var cpDto = new ChangePasswordDTO
                {
                    UserName = user.UserName + $"({user.Phone})",
                    UserId = id,
                    NewPassword = "123456"
                };
                return View(cpDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                ViewData["Error"] = "An error occurred";
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> ChangePass(ChangePasswordDTO model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.NewPassword) || string.IsNullOrEmpty(model.RePassword))
                {
                    ViewData["Error"] = "Vui lòng điền đủ thông tin!";
                    return View(model);
                }

                if (model.NewPassword != model.RePassword)
                {
                    ViewData["Error"] = "Mật khẩu nhập lại không khớp!";
                    return View(model);
                }

                var apiUrl = _configuration["ApiUrl"];
                using var client = _clientFactory.CreateClient();
                var response = await client.GetAsync($"{apiUrl}/users/{model.UserId}");

                if (!response.IsSuccessStatusCode)
                {
                    ViewData["Error"] = "Lấy dữ liệu người dùng không thành công!";
                    return View(model);
                }

                UserDTO uDto = await response.Content.ReadFromJsonAsync<UserDTO>();

                if (uDto == null || uDto != null && uDto.UserName == null)
                {
					var responseString = await response.Content.ReadAsStringAsync();
					ViewData["Error"] = responseString;
					//ViewData["Error"] = "Chuyển đổi dữ liệu người dùng không thành công!";
                    return View(model);
                }

                var data = new ChangePasswordDTO
                {
                    UserName = uDto.UserName,
                    NewPassword = model.NewPassword,
                };

                var json = JsonSerializer.Serialize(data);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var responsecp = await client.PostAsync($"{apiUrl}/users/changepass", content);

                if (responsecp.IsSuccessStatusCode)
                {
                    TempData["SuccessMsg"] = "Đổi mật khẩu thành công!";
                    return RedirectToAction("Index", "User");
                }
                else
                {
                    var responseStringCp = await responsecp.Content.ReadAsStringAsync();
                    ViewData["Error"] = responseStringCp;
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during work schedule creation");
                ViewData["Error"] = "An error occurred";
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Workschedule(int id, int? selectedWeek = null, int? selectedYear = null)
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];
                using var client = _clientFactory.CreateClient();
                var response = await client.GetAsync($"{apiUrl}/users/{id}");

                if (!response.IsSuccessStatusCode)
                {
                    ViewData["Error"] = "Lấy dữ liệu người dùng không thành công!";
                    return View("Error");
                }

                UserDTO uDto = await response.Content.ReadFromJsonAsync<UserDTO>();

                var employeeId = id;
                ViewData["employeeId"] = employeeId;

                // Nếu không có tuần nào được chọn, chọn tuần, năm hiện tại
                var currentWeek = selectedWeek ?? GetCurrentWeekOfYear();
                var currentYear = selectedYear ?? GetCurrentYear();

                // Lấy lịch làm việc
                var workScheduleResponse = await client.GetAsync($"{apiUrl}/work-schedules/current-user/?employeeId={employeeId}&weekNumber={currentWeek}&year={currentYear}");
                if (!workScheduleResponse.IsSuccessStatusCode)
                {
					var responseString = await workScheduleResponse.Content.ReadAsStringAsync();
					ViewData["Error"] = responseString;
					return View("Error");
				}
                var workSchedules = await workScheduleResponse.Content.ReadFromJsonAsync<IEnumerable<WorkScheduleDTO>>();

                var viewData = new CurrentWorkScheduleViewModel
                {
                    UserString = $"{uDto.UserName} ({uDto.FullName}) - ({uDto.Phone})",
                    SelectedYear = currentYear,
                    SelectedWeek = currentWeek,
                    WorkSchedules = workSchedules,
                };

                return View(viewData);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching work schedule for user");
                ViewData["Error"] = "An error occurred";
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

		public static string GenerateRandomString(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			char[] stringChars = new char[length];

			for (int i = 0; i < length; i++)
			{
				stringChars[i] = chars[new Random().Next(chars.Length)];
			}

			return new string(stringChars);
		}

		public static string RemoveDiacritics(string text)
		{
			var normalizedString = text.Normalize(NormalizationForm.FormD);
			var stringBuilder = new StringBuilder();

			foreach (var c in normalizedString)
			{
				var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
				if (unicodeCategory != UnicodeCategory.NonSpacingMark)
				{
					stringBuilder.Append(c);
				}
			}

			return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
		}
	}
}
