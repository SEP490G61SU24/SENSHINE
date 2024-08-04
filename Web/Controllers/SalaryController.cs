using API.Models;
using Microsoft.AspNetCore.Mvc;
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
            if (month.HasValue && year.HasValue)
            {
                response = _client.GetAsync(_client.BaseAddress + "/Salary/GetByMonthYear?month=" + month + "&year=" + year).Result;
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
            return View();
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
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(salary);
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
