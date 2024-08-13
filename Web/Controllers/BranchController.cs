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
        public async Task<IActionResult> ListBranch()
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            List<BranchViewModel> branchs = new List<BranchViewModel>();
            HttpResponseMessage response = client.GetAsync($"{apiUrl}/Branch/GetAll").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                branchs = JsonConvert.DeserializeObject<List<BranchViewModel>>(data);
                foreach (var branch in branchs)
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
                        Console.WriteLine("Error");
                    }
                }
            }

            return View(branchs);
        }

        [HttpGet]
        public async Task<IActionResult> CreateBranch()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateBranch(BranchViewModel branch)
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
                    ModelState.AddModelError(string.Empty, "Error");
                    return View(branch);
                }
            }

            return View(branch);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateBranch(int id)
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
                return NotFound("branch không tồn tại");
            }

            return View(branch);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateBranch(BranchViewModel branch)
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
                    ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật branch");
                    return View(branch);
                }
            }

            return View(branch);
        }
    }
}
