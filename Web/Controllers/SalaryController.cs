using API.Dtos;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Web.Models;

namespace Web.Controllers
{
    public class SalaryController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<UserController> _logger;

        public SalaryController(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<UserController> logger)
             : base(configuration, clientFactory, logger)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> ListSalary(int? month, int? year)
        {
            int? spaId = 1;
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                var userProfile = await GetUserProfileAsync(token);
                if (userProfile != null)
                {
                    spaId = userProfile.SpaId;
                }
                else
                {
                    ViewData["Error"] = "Failed to retrieve user profile.";
                }
            }
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            List<SalaryViewModel> salaries = new List<SalaryViewModel>();
            HttpResponseMessage response = null;
            if (month.HasValue && year.HasValue)
            {
                response = client.GetAsync($"{apiUrl}/Salary/GetByMonthYear?month=" + month + "&year=" + year).Result;
            }
            else
            {
                response = client.GetAsync($"{apiUrl}/Salary/GetAll").Result;
            }
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                salaries = JsonConvert.DeserializeObject<List<SalaryViewModel>>(data);
                HttpResponseMessage response2 = null;
                foreach (var item in salaries)
                {
                    response2 = client.GetAsync($"{apiUrl}/Branch/GetBranchByUser?id=" + item.EmployeeId).Result;
                    string data2 = response2.Content.ReadAsStringAsync().Result;
                    int BranchId = JsonConvert.DeserializeObject<int>(data2);
                    item.BranchId = BranchId;
                }
                salaries = salaries.Where(s => s.BranchId == spaId).ToList();

                foreach (var salary in salaries)
                {
                    HttpResponseMessage response1 = client.GetAsync($"{apiUrl}/user/" + salary.EmployeeId).Result;
                    if (response1.IsSuccessStatusCode)
                    {
                        string response1Body = response1.Content.ReadAsStringAsync().Result;
                        JObject json1 = JObject.Parse(response1Body);
                        salary.EmployeeName = json1["firstName"].ToString() + " " + json1["midName"].ToString() + " " + json1["lastName"].ToString();
                    }
                    else
                    {
                        Console.WriteLine("Error");
                    }
                }
            }

            return View(salaries);
        }

        [HttpGet]
        public async Task<IActionResult> CreateSalary()
        {
            int? spaId = 1;
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                var userProfile = await GetUserProfileAsync(token);
                if (userProfile != null)
                {
                    spaId = userProfile.SpaId;
                }
                else
                {
                    ViewData["Error"] = "Failed to retrieve user profile.";
                }
            }
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            var response1 = client.GetAsync($"{apiUrl}/user/byRole/3").Result;
            var response2 = client.GetAsync($"{apiUrl}/user/byRole/4").Result;
            if (response1.IsSuccessStatusCode && response2.IsSuccessStatusCode)
            {
                var users1 = response1.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>().Result;
                var users2 = response2.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>().Result;
                var combinedUsers = users1.Concat(users2);
                combinedUsers = combinedUsers.Where(u => u.SpaId == spaId).ToList();
                foreach (var user in combinedUsers)
                {
                    user.FullName = string.Join(" ", user.FirstName ?? "", user.MidName ?? "", user.LastName ?? "").Trim();
                    user.FullName = string.Join(", ", user.FullName ?? "", user.Phone ?? "").Trim();
                }
                ViewBag.Users = new SelectList(combinedUsers, "Id", "FullName");
                return View();
            }
            else
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSalary(SalaryViewModel salary)
        {
            int? spaId = 1;
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                var userProfile = await GetUserProfileAsync(token);
                if (userProfile != null)
                {
                    spaId = userProfile.SpaId;
                }
                else
                {
                    ViewData["Error"] = "Failed to retrieve user profile.";
                }
            }
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(salary);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync($"{apiUrl}/Salary/Create", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ListSalary");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Nhân viên này đã có lương tháng " + salary.SalaryMonth + " năm " + salary.SalaryYear);
                    var response1 = client.GetAsync($"{apiUrl}/user/byRole/3").Result;
                    var response2 = client.GetAsync($"{apiUrl}/user/byRole/4").Result;
                    if (response1.IsSuccessStatusCode && response2.IsSuccessStatusCode)
                    {
                        var users1 = response1.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>().Result;
                        var users2 = response2.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>().Result;
                        var combinedUsers = users1.Concat(users2);
                        combinedUsers = combinedUsers.Where(u => u.SpaId == spaId).ToList();
                        foreach (var user in combinedUsers)
                        {
                            user.FullName = string.Join(" ", user.FirstName ?? "", user.MidName ?? "", user.LastName ?? "").Trim();
                            user.FullName = string.Join(", ", user.FullName ?? "", user.Phone ?? "").Trim();
                        }
                        ViewBag.Users = new SelectList(combinedUsers, "Id", "FullName");
                        return View(salary);
                    }
                    else
                    {
                        return View("Error");
                    }
                }
            }

            return View(salary);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateSalary(int id)
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            SalaryViewModel salary = null;
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Salary/GetById?id=" + id);

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                salary = JsonConvert.DeserializeObject<SalaryViewModel>(data);
                HttpResponseMessage response1 = client.GetAsync($"{apiUrl}/user/" + salary.EmployeeId).Result;
                if (response1.IsSuccessStatusCode)
                {
                    string response1Body = response1.Content.ReadAsStringAsync().Result;
                    JObject json1 = JObject.Parse(response1Body);
                    salary.EmployeeName = json1["firstName"].ToString() + " " + json1["midName"].ToString() + " " + json1["lastName"].ToString();
                }
                else
                {
                    Console.WriteLine("Error");
                }
            }

            if (salary == null)
            {
                return NotFound("salary không tồn tại");
            }

            return View(salary);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSalary(SalaryViewModel salary)
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            SalaryCreateModel salaryCreate = new SalaryCreateModel();
            salaryCreate.Id = salary.Id;
            salaryCreate.EmployeeId = salary.EmployeeId;
            salaryCreate.BaseSalary = salary.BaseSalary;
            salaryCreate.Allowances = salary.Allowances;
            salaryCreate.Bonus = salary.Bonus;
            salaryCreate.Deductions = salary.Deductions;
            salaryCreate.TotalSalary = salary.TotalSalary;
            salaryCreate.SalaryMonth = salary.SalaryMonth;
            salaryCreate.SalaryYear = salary.SalaryYear;
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(salaryCreate);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync($"{apiUrl}/Salary/Update?id=" + salary.Id, content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ListSalary");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật salary");
                    return View(salary);
                }
            }

            return View(salary);
        }
    }
}
