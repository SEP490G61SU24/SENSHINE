using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Web.Models;

namespace Web.Controllers
{
    public class ServiceController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5297/api");
        private readonly HttpClient _httpClient;
        public ServiceController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }

        [HttpGet]
        public async Task<IActionResult> ListService(string searchString)
        {
            List<ServiceViewModel> servicesList = new List<ServiceViewModel>();

            try
            {
                // Gọi API để lấy danh sách dịch vụ
                HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/Service/GetAllServices");

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    servicesList = JsonConvert.DeserializeObject<List<ServiceViewModel>>(data);

                    // Lọc dữ liệu nếu có từ khóa tìm kiếm
                    if (!String.IsNullOrEmpty(searchString))
                    {
                        servicesList = servicesList.Where(s => s.ServiceName.Contains(searchString, StringComparison.OrdinalIgnoreCase)).ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                // Xử lý lỗi khi gọi API
                ModelState.AddModelError(string.Empty, $"Có lỗi xảy ra: {ex.Message}");
            }

            return View(servicesList);
        }
        // GET: Service/Details/{id}
        [HttpGet]
        public async Task<IActionResult> DetailService(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID Service không hợp lệ");
            }

            ServiceViewModel service = null;
            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + $"/Service/GetByID?Id={id}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                service = JsonConvert.DeserializeObject<ServiceViewModel>(data);
            }

            if (service == null)
            {
                return NotFound("Không tìm thấy dịch vụ");
            }

            return View(service);
        }
        //Add New Service
        [HttpGet]
        public IActionResult CreateService()
        {
            return View();
        }
        // POST: Service/Create
        [HttpPost]
        public async Task<IActionResult> CreateService(ServiceViewModel service)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(service);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(_httpClient.BaseAddress + $"/Service/Create", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ListService");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi tạo mới dịch vụ.");
                    return View(service);
                }
            }

            return View(service);
        }
        //Edit Service
        [HttpGet]
        public async Task<IActionResult> EditService(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID Service không hợp lệ");
            }

            ServiceViewModel service = null;
            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + $"/Service/GetByID?Id={id}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                service = JsonConvert.DeserializeObject<ServiceViewModel>(data);
            }

            if (service == null)
            {
                return NotFound("Không tìm thấy dịch vụ");
            }

            return View(service);
        }

        [HttpPost]
        public async Task<IActionResult> EditService(ServiceViewModel service)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(service);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PutAsync(_httpClient.BaseAddress + $"/Service/UpdateService?id={service.Id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ListService");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật dịch vụ.");
                    return View(service);
                }
            }

            return View(service);
        }

        // POST: Service/DeleteService/{id}
        [HttpPost]
        public async Task<IActionResult> DeleteService1(int id)
        {
            if (id <= 0)
            {
                return BadRequest("ID Service không hợp lệ");
            }

            HttpResponseMessage response = await _httpClient.DeleteAsync(_httpClient.BaseAddress + $"/Service/DeleteService/delete/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListService");
            }
            else
            {
                return BadRequest("Có lỗi xảy ra khi xóa dịch vụ.");
            }
        }
    }
}
