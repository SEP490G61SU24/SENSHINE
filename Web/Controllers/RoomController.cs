using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using Web.Models;
using API.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Web.Controllers
{
    public class RoomController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5297/api");
        private readonly HttpClient _client;

        public RoomController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }
        [HttpGet]
        public async Task<IActionResult> ListRoom()
        {
            List<RoomViewModel> rooms = new List<RoomViewModel>();
            List<BranchViewModel> branches = new List<BranchViewModel>();
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Room/GetAll").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                rooms = JsonConvert.DeserializeObject<List<RoomViewModel>>(data);
                foreach (var room in rooms)
                {
                    HttpResponseMessage response1 = _client.GetAsync(_client.BaseAddress + "/Branch/GetById?id=" + room.SpaId).Result;
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

        [HttpGet]
        public IActionResult CreateRoom()
        {
            HttpResponseMessage response1 = _client.GetAsync(_client.BaseAddress + "/Branch/GetAll").Result;
            if (response1.IsSuccessStatusCode)
            {
                var branches = response1.Content.ReadFromJsonAsync<IEnumerable<BranchViewModel>>().Result;
                ViewBag.Spas = new SelectList(branches, "Id", "SpaName");
            }
            else
            {
                Console.WriteLine("Error");
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateRoom(RoomViewModel room)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(room);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + "/Room/Create", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ListRoom");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error");
                    return View(room);
                }
            }

            return View(room);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateRoom(int id)
        {
            RoomViewModel room = null;
            HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress + "/Room/GetById?id=" + id);

            if (response.IsSuccessStatusCode)
            {
                HttpResponseMessage response1 = _client.GetAsync(_client.BaseAddress + "/Branch/GetAll").Result;
                if (response1.IsSuccessStatusCode)
                {
                    var branches = response1.Content.ReadFromJsonAsync<IEnumerable<BranchViewModel>>().Result;
                    ViewBag.Spas = new SelectList(branches, "Id", "SpaName");
                }
                else
                {
                    Console.WriteLine("Error");
                }
                string data = await response.Content.ReadAsStringAsync();
                room = JsonConvert.DeserializeObject<RoomViewModel>(data);
            }

            if (room == null)
            {
                return NotFound("room không tồn tại");
            }

            return View(room);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateRoom(RoomViewModel room)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(room);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PutAsync(_client.BaseAddress + "/Room/Update?id=" + room.Id, content);

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
    }
}
