using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using Web.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using API.Ultils;
using Microsoft.EntityFrameworkCore;
using API.Models;

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
        public async Task<IActionResult> ListRoom(int pageIndex = 1, int pageSize = 10, string searchTerm = null)
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];
                var url = $"{apiUrl}/Room/GetAll?pageIndex={pageIndex}&pageSize={pageSize}&searchTerm={searchTerm}";
                var client = _clientFactory.CreateClient();
                PaginatedList<RoomViewModel> rooms = new PaginatedList<RoomViewModel>();
                List<BranchViewModel> branches = new List<BranchViewModel>();
                HttpResponseMessage response = client.GetAsync(url).Result;
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
                        ViewData["Error"] = "Không lấy được dữ liệu của người dùng hiện tại";
                    }
                }

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    rooms = JsonConvert.DeserializeObject<PaginatedList<RoomViewModel>>(data);

                    // Convert to a list to apply LINQ filters
                    var filteredRooms = rooms.Items.Where(r => r.SpaId == spaId).ToList();

                    // Re-assign filtered cards back to the PaginatedList if necessary
                    rooms.Items = filteredRooms;

                    foreach (var room in rooms.Items)
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
                            ViewData["Error"] = "Có lỗi xảy ra";
                        }
                    }
                }

                return View(rooms);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");
                ViewData["Error"] = "Có lỗi xảy ra";
                return View("Error");
            }
        }

        public async Task<IActionResult> GetBeds(int roomId)
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Bed/GetByRoomId/ByRoomId/" + roomId);

                if (response.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    var beds = JsonConvert.DeserializeObject<List<BedViewModel>>(data);

                    return PartialView("_BedsPartial", beds);
                }

                ViewData["Error"] = "Không lấy được dữ liệu phòng";
                return View("Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");
                ViewData["Error"] = "Có lỗi xảy ra";
                return View("Error");
            }
        }


        [HttpGet]
        public async Task<IActionResult> DetailRoom(int id)
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();
                RoomViewModel room = new RoomViewModel();
                List<BedViewModel> beds = new List<BedViewModel>();

                HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Room/GetById?id=" + id);
                HttpResponseMessage response1 = await client.GetAsync($"{apiUrl}/Bed/GetByRoomId/ByRoomId/" + id);

                if (response.IsSuccessStatusCode && response1.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    string data1 = await response1.Content.ReadAsStringAsync();
                    room = JsonConvert.DeserializeObject<RoomViewModel>(data);
                    beds = JsonConvert.DeserializeObject<List<BedViewModel>>(data1);

                    HttpResponseMessage response2 = client.GetAsync($"{apiUrl}/Branch/GetById?id=" + room.SpaId).Result;
                    if (response2.IsSuccessStatusCode)
                    {
                        string response2Body = response2.Content.ReadAsStringAsync().Result;
                        JObject json2 = JObject.Parse(response2Body);
                        room.SpaName = json2["spaName"].ToString();
                    }
                    else
                    {
                        ViewData["Error"] = "Có lỗi xảy ra";
                    }
                }

                if (room == null)
                {
                    ViewData["Error"] = "Không tìm thấy phòng";
                }

                var roomViewModel = new RoomViewModel
                {
                    Id = room.Id,
                    RoomName = room.RoomName,
                    SpaName = room.SpaName,
                    Beds = beds.Select(b => new BedViewModel
                    {
                        BedNumber = b.BedNumber,
                        StatusWorking = b.StatusWorking
                    }).ToList()
                };

                return View(roomViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");
                ViewData["Error"] = "Có lỗi xảy ra";
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
                _logger.LogError(ex, "Error");
                ViewData["Error"] = "Có lỗi xảy ra";
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
                        ViewData["Error"] = "Không lấy được dữ liệu của người dùng hiện tại";
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
                        ViewData["Error"] = "Chi nhánh không tồn tại";
                        return View(room);
                    }

                }

                return View(room);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");
                ViewData["Error"] = "Có lỗi xảy ra";
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
                    ViewData["Error"] = "phòng không tồn tại";
                    return NotFound();
                }

                return View(room);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");
                ViewData["Error"] = "Có lỗi xảy ra";
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
                        ViewData["Error"] = "Không lấy được dữ liệu của người dùng hiện tại";
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
                        ViewData["Error"] = "Có lỗi xảy ra khi cập nhật phòng";
                        return View(room);
                    }
                }

                return View(room);
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
