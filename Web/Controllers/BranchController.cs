using API.Ultils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Web.Models;

namespace Web.Controllers
{
    public class BranchController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<UserController> _logger;

        public BranchController(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<UserController> logger)
             : base(configuration, clientFactory, logger)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> ListBranch(int pageIndex = 1, int pageSize = 10, string searchTerm = null)
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];
                var url = $"{apiUrl}/Branch/GetAll?pageIndex={pageIndex}&pageSize={pageSize}&searchTerm={searchTerm}";
                var client = _clientFactory.CreateClient();
                PaginatedList<BranchViewModel> branches = new PaginatedList<BranchViewModel>();
                HttpResponseMessage response = client.GetAsync(url).Result;

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    branches = JsonConvert.DeserializeObject<PaginatedList<BranchViewModel>>(data);

                    foreach (var branch in branches.Items)
                    {
                        HttpResponseMessage response1 = client.GetAsync($"{apiUrl}/provinces/" + branch.ProvinceCode).Result;
                        HttpResponseMessage response2 = client.GetAsync($"{apiUrl}/districts/" + branch.DistrictCode).Result;
                        HttpResponseMessage response3 = client.GetAsync($"{apiUrl}/wards/" + branch.WardCode).Result;
                        if (response1.IsSuccessStatusCode && response2.IsSuccessStatusCode && response3.IsSuccessStatusCode)
                        {
                            string response1Body = response1.Content.ReadAsStringAsync().Result;
                            string response2Body = response2.Content.ReadAsStringAsync().Result;
                            string response3Body = response3.Content.ReadAsStringAsync().Result;
                            JObject json1 = JObject.Parse(response1Body);
                            JObject json2 = JObject.Parse(response2Body);
                            JObject json3 = JObject.Parse(response3Body);
                            branch.ProvinceName = json1["name"].ToString();
                            branch.DistrictName = json2["name"].ToString();
                            branch.WardName = json3["name"].ToString();
                        }
                        else
                        {
                            ViewData["Error"] = "Có lỗi xảy ra";
                        }
                    }
                }

                return View(branches);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");
                ViewData["Error"] = "Có lỗi xảy ra";
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateBranch()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");
                ViewData["Error"] = "Có lỗi xảy ra";
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateBranch(BranchViewModel branch)
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();
                if (ModelState.IsValid)
                {
                    var json = JsonConvert.SerializeObject(branch);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync($"{apiUrl}/Branch/Create", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("ListBranch");
                    }
                    else
                    {
                        ViewData["Error"] = "Có lỗi xảy ra";
                        return View(branch);
                    }
                }

                return View(branch);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");
                ViewData["Error"] = "Có lỗi xảy ra";
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateBranch(int id)
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();
                BranchViewModel branch = null;
                HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Branch/GetById?id=" + id);

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    branch = JsonConvert.DeserializeObject<BranchViewModel>(data);
                }

                if (branch == null)
                {
                    ViewData["Error"] = "chi nhánh không tồn tại";
                    return NotFound();
                }

                return View(branch);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");
                ViewData["Error"] = "Có lỗi xảy ra";
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBranch(BranchViewModel branch)
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();
                if (ModelState.IsValid)
                {
                    var json = JsonConvert.SerializeObject(branch);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync($"{apiUrl}/Branch/Update?id=" + branch.Id, content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("ListBranch");
                    }
                    else
                    {
                        ViewData["Error"] = "Có lỗi xảy ra khi cập nhật chi nhánh";
                        return View(branch);
                    }
                }

                return View(branch);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");
                ViewData["Error"] = "Có lỗi xảy ra";
                return View("Error");
            }
        }
    }
}
