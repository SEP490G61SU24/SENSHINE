using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;

namespace Web.Controllers
{
    public class RoomController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<UserController> _logger;

        public RoomController(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<UserController> logger)
             : base(configuration, clientFactory, logger)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> ListRoom()
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();
                List<RoomViewModel> rooms = new List<RoomViewModel>();
                List<BranchViewModel> branches = new List<BranchViewModel>();
                HttpResponseMessage response = client.GetAsync($"{apiUrl}/Room/GetAll").Result;
                int? spaId = 0;
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

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    rooms = JsonConvert.DeserializeObject<List<RoomViewModel>>(data);
                    rooms = rooms.Where(r => r.SpaId == spaId).ToList();
                    foreach (var room in rooms)
                    {
                        HttpResponseMessage response1 = client.GetAsync($"{apiUrl}/Branch/GetById?id=" + room.SpaId).Result;
                        if (response1.IsSuccessStatusCode)
                        {
                            string response1Body = response1.Content.ReadAsStringAsync().Result;
                            JObject json1 = JObject.Parse(response1Body);
                            room.SpaName = json1["spaName"].ToString();
                        }
                        else
                        {
                            Console.WriteLine("Error");
                        }
                    }
                }

                return View(rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra.");
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateRoom()
        {
            try
            {
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra.");
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom(RoomViewModel room)
        {
            try
            {
                int? spaId = 0;
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
                room.SpaId = spaId;
                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();

                if (ModelState.IsValid)
                {
                    var json = JsonConvert.SerializeObject(room);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PostAsync($"{apiUrl}/Room/Create", content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("ListRoom");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Chi nhánh không tồn tại");

                        return View(room);
                    }

                }

                return View(room);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra.");
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateRoom(int id)
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();
                RoomViewModel room = null;
                HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Room/GetById?id=" + id);

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    room = JsonConvert.DeserializeObject<RoomViewModel>(data);
                }

                if (room == null)
                {
                    return NotFound("room không tồn tại");
                }

                return View(room);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra.");
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRoom(RoomViewModel room)
        {
            try
            {
                int? spaId = 0;
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
                room.SpaId = spaId;
                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();

                if (ModelState.IsValid)
                {
                    var json = JsonConvert.SerializeObject(room);
                    var content = new StringContent(json, Encoding.UTF8, "application/json");

                    HttpResponseMessage response = await client.PutAsync($"{apiUrl}/Room/Update?id=" + room.Id, content);

                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction("ListRoom");
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật room");
                        return View(room);
                    }
                }

                return View(room);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Có lỗi xảy ra.");
                return View("Error");
            }
        }
    }
}
