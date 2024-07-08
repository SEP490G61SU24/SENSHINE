using API.Ultils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Web.Models;

namespace Web.Controllers
{
    public class EmployeeController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5297/api");
        private readonly HttpClient _client;

        public EmployeeController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }

        [HttpGet]
        public async Task<IActionResult> ListEmployee()
        {
            List<EmployeeViewModel> employees = new List<EmployeeViewModel>();
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Employee/GetAll").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                employees = JsonConvert.DeserializeObject<List<EmployeeViewModel>>(data);
            }

            return View(employees);
        }

        [HttpGet]
        public IActionResult CreateEmployee()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateEmployee(EmployeeViewModel employee)
        {
            string date = employee.BirthDate.Value.ToString("yyyy-MM-dd");
            DateTime date2 = FormatDateTimeUtils.ParseDateTimeLikeSSMS(date);
            employee.BirthDate = date2;

            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(employee);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + "/Employee/Create", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ListEmployee");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error");
                    return View(employee);
                }
            }

            return View(employee);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateEmployee(int id)
        {
            EmployeeViewModel employee = null;
            HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress + "/Employee/GetById?id=" + id);

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                employee = JsonConvert.DeserializeObject<EmployeeViewModel>(data);
            }

            if (employee == null)
            {
                return NotFound("User không tồn tại");
            }

            return View(employee);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateEmployee(EmployeeViewModel employee)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(employee);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PutAsync(_client.BaseAddress + "/Employee/Update?id=" + employee.Id, content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ListEmployee");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật user");
                    return View(employee);
                }
            }

            return View(employee);
        }
    }
}
