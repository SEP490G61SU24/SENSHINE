using API.Ultils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Web.Models;

namespace Web.Controllers
{
    public class ServiceController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<UserController> _logger;

        public ServiceController(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<UserController> logger)
             : base(configuration, clientFactory, logger)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> ListService(int pageIndex = 1, int pageSize = 10, string searchTerm = null)
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];
                var url = $"{apiUrl}/Service/GetAll?pageIndex={pageIndex}&pageSize={pageSize}&searchTerm={searchTerm}";
                var client = _clientFactory.CreateClient();
                PaginatedList<ServiceViewModel> services = new PaginatedList<ServiceViewModel>();
                HttpResponseMessage response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    services = JsonConvert.DeserializeObject<PaginatedList<ServiceViewModel>>(data);
                    return View(services);
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

        // GET: Service/Details/{id}
        [HttpGet]
        public async Task<IActionResult> DetailService(int id)
        {
            try
            {
                var client = _clientFactory.CreateClient();
                var apiUrl = _configuration["ApiUrl"];
                if (id <= 0)
                {
                    return BadRequest("ID Service không hợp lệ");
                }

                ServiceViewModel service = null;
                HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Service/GetByID?Id={id}");

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
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
            try
            {
                var client = _clientFactory.CreateClient();
                var apiUrl = _configuration["ApiUrl"];
                if (ModelState.IsValid)
                {
                    var json = JsonConvert.SerializeObject(service);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync($"{apiUrl}/Service/Create", content);

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }

        //Edit Service
        [HttpGet]
        public async Task<IActionResult> EditService(int id)
        {
            try
            {
                var client = _clientFactory.CreateClient();
                var apiUrl = _configuration["ApiUrl"];
                if (id <= 0)
                {
                    return BadRequest("ID Service không hợp lệ");
                }

                ServiceViewModel service = null;
                HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Service/GetByID?Id={id}");

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> EditService(ServiceViewModel service)
        {
            try
            {
                var client = _clientFactory.CreateClient();
                var apiUrl = _configuration["ApiUrl"];
                if (ModelState.IsValid)
                {
                    var json = JsonConvert.SerializeObject(service);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync($"{apiUrl}/Service/UpdateService?id={service.Id}", content);

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
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }

        // POST: Service/DeleteService/{id}
        [HttpPost]
        public async Task<IActionResult> DeleteService1(int id)
        {
            try
            {
                var client = _clientFactory.CreateClient();
                var apiUrl = _configuration["ApiUrl"];
                if (id <= 0)
                {
                    return BadRequest("ID Service không hợp lệ");
                }

                HttpResponseMessage response = await client.DeleteAsync($"{apiUrl}/Service/DeleteService/delete/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ListService");
                }
                else
                {
                    return BadRequest("Có lỗi xảy ra khi xóa dịch vụ.");
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
