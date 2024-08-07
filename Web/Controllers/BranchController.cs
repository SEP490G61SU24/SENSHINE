using API.Ultils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text;
using Web.Models;

namespace Web.Controllers
{
    public class BranchController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5297/api");
        private readonly HttpClient _client;

        public BranchController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }
        [HttpGet]
        public async Task<IActionResult> ListBranch()
        {
            List<BranchViewModel> branchs = new List<BranchViewModel>();
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Branch/GetAll").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                branchs = JsonConvert.DeserializeObject<List<BranchViewModel>>(data);
                foreach (var branch in branchs)
                {
                    HttpResponseMessage response1 = _client.GetAsync(_client.BaseAddress + "/provinces/" + branch.ProvinceCode).Result;
                    HttpResponseMessage response2 = _client.GetAsync(_client.BaseAddress + "/districts/" + branch.DistrictCode).Result;
                    HttpResponseMessage response3 = _client.GetAsync(_client.BaseAddress + "/wards/" + branch.WardCode).Result;
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
        public IActionResult CreateBranch()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateBranch(BranchViewModel branch)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(branch);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + "/Branch/Create", content);

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
            BranchViewModel branch = null;
            HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress + "/Branch/GetById?id=" + id);

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
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(branch);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PutAsync(_client.BaseAddress + "/Branch/Update?id=" + branch.Id, content);

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
