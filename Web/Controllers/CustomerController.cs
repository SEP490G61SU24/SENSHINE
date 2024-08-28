using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using API.Dtos;
using API.Ultils;
using Web.Utils;

namespace Web.Controllers
{
	public class CustomerController : BaseController
	{
		private readonly IConfiguration _configuration;
		private readonly IHttpClientFactory _clientFactory;
		private readonly ILogger<CustomerController> _logger;

		public CustomerController(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<CustomerController> logger)
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

                var url = $"{apiUrl}/users/page/role/{(int)UserRoleEnum.CUSTOMER}?pageIndex={pageIndex}&pageSize={pageSize}&searchTerm={searchTerm}&spaId={spaId}";

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
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
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
				user.Password = "SenShine123@";

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

				user.UserName = (StringUtils.RemoveDiacritics(user.LastName) + user.ProvinceCode + StringUtils.GenerateRandomString(4)).ToLower();
                user.RoleId = (int) UserRoleEnum.CUSTOMER;
                user.SpaId = ViewData["SpaId"] != null && ViewData["SpaId"].ToString() != "ALL"
                            ? int.Parse(ViewData["SpaId"].ToString())
                            : (int?)null;

                var apiUrl = _configuration["ApiUrl"];

				var json = JsonSerializer.Serialize(user);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				using var client = _clientFactory.CreateClient();
				var response = await client.PostAsync($"{apiUrl}/users", content);

				if (response.IsSuccessStatusCode)
				{
                    TempData["SuccessMsg"] = "Thêm thành công!";
                    return RedirectToAction("Index", "Customer");
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
                var response = await client.GetAsync($"{apiUrl}/users/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadFromJsonAsync<UserDTO>();
                    if (user.RoleId != (int)UserRoleEnum.CUSTOMER)
                    {
                        return View("~/Views/Errors/404.cshtml");
                    }
                    return View(user);
                }
                else
                {
                    if(response.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return View("~/Views/Errors/404.cshtml");
                    }
                    var responseString = await response.Content.ReadAsStringAsync();
                    ViewData["Error"] = responseString;
                    return View("~/Views/Errors/500.cshtml");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
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

                user.RoleId = 5; // CUSTOMER

                var apiUrl = _configuration["ApiUrl"];

                var json = JsonSerializer.Serialize(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var client = _clientFactory.CreateClient();
                var response = await client.PutAsync($"{apiUrl}/users/{user.Id}", content);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMsg"] = "Cập nhật thành công!";
                    return RedirectToAction("Index", "Customer");
                }
                else
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    ViewData["Error"] = responseString;
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

        [HttpGet]
        public async Task<IActionResult> Appointment(int id)
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();
                var response = await client.GetAsync($"{apiUrl}/Appointment/GetByCustomerId/customer/{id}");

                var responseUser = await client.GetAsync($"{apiUrl}/users/{id}");
                if (responseUser.IsSuccessStatusCode)
                {
                    var user = await responseUser.Content.ReadFromJsonAsync<UserDTO>();
                    if (user.RoleId != (int)UserRoleEnum.CUSTOMER)
                    {
                        return View("~/Views/Errors/404.cshtml");
                    }
                    ViewData["CusDetail"] = $"{user.FullName}  ({user.Phone})";
                }
                else
                {
                    if (responseUser.StatusCode == System.Net.HttpStatusCode.NotFound)
                    {
                        return View("~/Views/Errors/404.cshtml");
                    }
                    var responseUserString = await responseUser.Content.ReadAsStringAsync();
                    ViewData["Error"] = responseUserString;
                    return View("~/Views/Errors/500.cshtml");
                }

                if (response.IsSuccessStatusCode)
                {
                    if (response.StatusCode == System.Net.HttpStatusCode.NoContent)
                    {
                        var appointments = new List<AppointmentDTO>();
                        return View(appointments);
                    }
                    else
                    {
                        var appointments = await response.Content.ReadFromJsonAsync<List<AppointmentDTO>>();
                        return View(appointments);
                    }
                }
                else
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    ViewData["Error"] = responseString;
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
    }
}
