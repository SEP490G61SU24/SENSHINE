using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Controllers
{
    public class BedController : Controller
    {
        private readonly HttpClient _httpClient;

        public BedController()
        {
            _httpClient = new HttpClient { BaseAddress = new Uri("http://localhost:5297/api") };
        }

        public async Task<IActionResult> Index()
        {
            List<BedViewModel> bedList = new List<BedViewModel>();

            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/Bed/GetAllBeds");

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                bedList = JsonConvert.DeserializeObject<List<BedViewModel>>(jsonString);
            }
            return View(bedList);
        }

        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var rooms = await GetAvailableRooms();
            ViewBag.Rooms = rooms;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(BedViewModel bedViewModel)
        {
            if (!ModelState.IsValid)
            {
                var rooms = await GetAvailableRooms();
                ViewBag.Rooms = rooms;
                return View(bedViewModel);
            }

            string jsonString = JsonConvert.SerializeObject(bedViewModel);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/Bed/AddBed", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            string errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, errorMessage);
            var roomsList = await GetAvailableRooms();
            ViewBag.Rooms = roomsList;
            return View(bedViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var bed = await GetBedById(id);
            if (bed == null)
            {
                return NotFound();
            }

            var rooms = await GetAvailableRooms();
            ViewBag.Rooms = rooms;

            return View(bed);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BedViewModel bedViewModel)
        {
            if (!ModelState.IsValid)
            {
                var rooms = await GetAvailableRooms();
                ViewBag.Rooms = rooms;
                return View(bedViewModel);
            }

            string jsonString = JsonConvert.SerializeObject(bedViewModel);
            var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync(_httpClient.BaseAddress + $"/Bed/UpdateBed/{bedViewModel.Id}", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("Index");
            }

            string errorMessage = await response.Content.ReadAsStringAsync();
            ModelState.AddModelError(string.Empty, errorMessage);
            var roomsList = await GetAvailableRooms();
            ViewBag.Rooms = roomsList;
            return View(bedViewModel);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/Bed/DeleteBed/{id}");

                if (response.IsSuccessStatusCode)
                {
                    return Json(new { success = true });
                }

                return Json(new { success = false, message = "An error occurred while deleting the bed." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An unexpected error occurred." });
            }
        }

        private async Task<List<RoomViewModel>> GetAvailableRooms()
        {
            List<RoomViewModel> rooms = new List<RoomViewModel>();

            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/Room/GetAll");

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                rooms = JsonConvert.DeserializeObject<List<RoomViewModel>>(jsonString);
            }

            return rooms;
        }

        private async Task<BedViewModel> GetBedById(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + $"/Bed/GetBedById/{id}");

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<BedViewModel>(jsonString);
            }

            return null;
        }
    }
}
