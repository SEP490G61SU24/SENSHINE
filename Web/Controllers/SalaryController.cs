﻿using API.Dtos;
using API.Models;
using API.Ultils;
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
        public async Task<IActionResult> ListSalary(int month, int year, int employee, int pageIndex = 1, int pageSize = 10, string searchTerm = null)
        {
            try
            {
                int? spaId = ViewData["SpaId"] != null && ViewData["SpaId"].ToString() != "ALL"
                ? int.Parse(ViewData["SpaId"].ToString())
                : (int?)null;
                var apiUrl = _configuration["ApiUrl"];
                var url = $"{apiUrl}/Salary/GetAll?pageIndex={pageIndex}&pageSize={pageSize}&searchTerm={searchTerm}&spaId={spaId}";
                var client = _clientFactory.CreateClient();
                PaginatedList<SalaryViewModel> salaries = new PaginatedList<SalaryViewModel>();
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    salaries = JsonConvert.DeserializeObject<PaginatedList<SalaryViewModel>>(data);
                    HttpResponseMessage response1 = null;
                    HttpResponseMessage response2 = null;
                    foreach (var salary in salaries.Items)
                    {
                        response1 = client.GetAsync($"{apiUrl}/users/" + salary.EmployeeId).Result;
                        string response1Body = response1.Content.ReadAsStringAsync().Result;
                        JObject json1 = JObject.Parse(response1Body);
                        salary.EmployeeName = json1["firstName"].ToString() + " " + json1["midName"].ToString() + " " + json1["lastName"].ToString();
                        response2 = client.GetAsync($"{apiUrl}/Branch/GetById?id=" + Int32.Parse(json1["spaId"].ToString())).Result;
                        string response2Body = response2.Content.ReadAsStringAsync().Result;
                        JObject json2 = JObject.Parse(response2Body);
                        salary.BranchName = json2["spaName"].ToString();
                    }

                    var response3 = client.GetAsync($"{apiUrl}/users/role/2").Result;
                    var response4 = client.GetAsync($"{apiUrl}/users/role/3").Result;
                    var response5 = client.GetAsync($"{apiUrl}/users/role/4").Result;
                    if (response3.IsSuccessStatusCode && response4.IsSuccessStatusCode && response5.IsSuccessStatusCode)
                    {
                        var users3 = response3.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>().Result;
                        var users4 = response4.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>().Result;
                        var users5 = response5.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>().Result;
                        var combinedUsers = users3.Concat(users4);
                        combinedUsers = combinedUsers.Concat(users5);

                        if (spaId != null)
                        {
                            combinedUsers = combinedUsers.Where(u => u.SpaId == spaId).ToList();
                        }

                        foreach (var user in combinedUsers)
                        {
                            user.FullName = string.Join(" ", user.FirstName ?? "", user.MidName ?? "", user.LastName ?? "").Trim();
                            user.FullName = string.Join(", ", user.FullName ?? "", user.Phone ?? "").Trim();
                        }

                        ViewBag.Users = combinedUsers;
                    }
                    else
                    {
                        ViewData["Error"] = "Có lỗi xảy ra";
                    }

                    if (!month.Equals(0) && !year.Equals(0))
                    {
                        salaries.Items = salaries.Items.Where(s => s.SalaryMonth == month && s.SalaryYear == year).ToList();
                    }

                    if (!employee.Equals(0))
                    {
                        salaries.Items = salaries.Items.Where(s => s.EmployeeId.Equals(employee)).ToList();
                    }
                }

                return View(salaries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateSalary()
        {
            try
            {
                int? spaId = ViewData["SpaId"] != null && ViewData["SpaId"].ToString() != "ALL"
                ? int.Parse(ViewData["SpaId"].ToString())
                : (int?)null;
                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();
                var response1 = client.GetAsync($"{apiUrl}/users/role/2").Result;
                var response2 = client.GetAsync($"{apiUrl}/users/role/3").Result;
                var response3 = client.GetAsync($"{apiUrl}/users/role/4").Result;
                if (response1.IsSuccessStatusCode && response2.IsSuccessStatusCode)
                {
                    var users1 = response1.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>().Result;
                    var users2 = response2.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>().Result;
                    var users3 = response3.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>().Result;
                    var combinedUsers = users1.Concat(users2);
                    combinedUsers = combinedUsers.Concat(users3);

                    if (spaId != null)
                    {
                        combinedUsers = combinedUsers.Where(u => u.SpaId == spaId).ToList();
                    }

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateSalary(SalaryViewModel salary)
        {
            try
            {
                int? spaId = ViewData["SpaId"] != null && ViewData["SpaId"].ToString() != "ALL"
                ? int.Parse(ViewData["SpaId"].ToString())
                : (int?)null;
                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();

                var response1 = client.GetAsync($"{apiUrl}/users/role/2").Result;
                var response2 = client.GetAsync($"{apiUrl}/users/role/3").Result;
                var response3 = client.GetAsync($"{apiUrl}/users/role/4").Result;
                if (response1.IsSuccessStatusCode && response2.IsSuccessStatusCode && response3.IsSuccessStatusCode)
                {
                    var users1 = response1.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>().Result;
                    var users2 = response2.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>().Result;
                    var users3 = response3.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>().Result;
                    var combinedUsers = users1.Concat(users2);
                    combinedUsers = combinedUsers.Concat(users3);

                    if (spaId != null)
                    {
                        combinedUsers = combinedUsers.Where(u => u.SpaId == spaId).ToList();
                    }

                    foreach (var user in combinedUsers)
                    {
                        user.FullName = string.Join(" ", user.FirstName ?? "", user.MidName ?? "", user.LastName ?? "").Trim();
                        user.FullName = string.Join(", ", user.FullName ?? "", user.Phone ?? "").Trim();
                    }
                    ViewBag.Users = new SelectList(combinedUsers, "Id", "FullName");
                }
                else
                {
                    return View("Error");
                }

                if (ModelState.IsValid)
                {
                    var json = JsonConvert.SerializeObject(salary);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync($"{apiUrl}/Salary/Create", content);

                    if (response.IsSuccessStatusCode)
                    {
                        TempData["SuccessMsg"] = "Thêm thành công!";
                        return RedirectToAction("ListSalary");
                    }
                    else
                    {
                        ViewData["Error"] = "Nhân viên này không tồn tại hoặc đã có lương tháng " + salary.SalaryMonth + " năm " + salary.SalaryYear;
                        return View(salary);
                    }
                }

                return View(salary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateSalary(int id)
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();
                SalaryViewModel salary = null;
                HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Salary/GetById?id=" + id);

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    salary = JsonConvert.DeserializeObject<SalaryViewModel>(data);
                    HttpResponseMessage response1 = client.GetAsync($"{apiUrl}/users/" + salary.EmployeeId).Result;
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
                    ViewData["Error"] = "lương không tồn tại";
                    return NotFound();
                }

                return View(salary);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSalary(SalaryViewModel salary)
        {
            try
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
                        TempData["SuccessMsg"] = "Cập nhật thành công!";
                        return RedirectToAction("ListSalary");
                    }
                    else
                    {
                        ViewData["Error"] = "Có lỗi xảy ra khi cập nhật lương";
                        return View(salary);
                    }
                }

                return View(salary);
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
