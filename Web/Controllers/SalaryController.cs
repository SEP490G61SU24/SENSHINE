using API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Web.Models;

namespace Web.Controllers
{
    public class SalaryController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5297/api");
        private readonly HttpClient _client;

        public SalaryController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }
        [HttpGet]
        public async Task<IActionResult> ListSalary(int? month, int? year)
        {
            List<SalaryViewModel> salaries = new List<SalaryViewModel>();
            HttpResponseMessage response = null;
            if (month.HasValue && year.HasValue)
            {
                response = _client.GetAsync(_client.BaseAddress + "/Salary/GetByMonthYear?month=" + month + "&year=" + year).Result;
            }
            else
            {
                response = _client.GetAsync(_client.BaseAddress + "/Salary/GetAll").Result;
            }
            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                salaries = JsonConvert.DeserializeObject<List<SalaryViewModel>>(data);
                foreach (var salary in salaries)
                {
                    HttpResponseMessage response1 = _client.GetAsync(_client.BaseAddress + "/user/" + salary.EmployeeId).Result;
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
        public IActionResult CreateSalary()
        {
            var response1 = _client.GetAsync($"http://localhost:5297/api/user/byRole/3").Result;
            var response2 = _client.GetAsync($"http://localhost:5297/api/user/byRole/4").Result;
            if (response1.IsSuccessStatusCode && response2.IsSuccessStatusCode)
            {
                var users1 = response1.Content.ReadFromJsonAsync<IEnumerable<UserViewModel>>().Result;
                var users2 = response2.Content.ReadFromJsonAsync<IEnumerable<UserViewModel>>().Result;
                var combinedUsers = users1.Concat(users2);
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

            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(salary);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + "/Salary/Create", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ListSalary");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error");
                    return View(salary);
                }
            }

            return View(salary);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateSalary(int id)
        {
            SalaryViewModel salary = null;
            HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress + "/Salary/GetById?id=" + id);

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                salary = JsonConvert.DeserializeObject<SalaryViewModel>(data);
                HttpResponseMessage response1 = _client.GetAsync(_client.BaseAddress + "/user/" + salary.EmployeeId).Result;
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

                HttpResponseMessage response = await _client.PutAsync(_client.BaseAddress + "/Salary/Update?id=" + salary.Id, content);

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
