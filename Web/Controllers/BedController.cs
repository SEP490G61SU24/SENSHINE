using API.Dtos;
using API.Ultils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Web.Models;

namespace Web.Controllers
{
    public class BedController :  BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<UserController> _logger;

        public BedController(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<UserController> logger) : base(configuration, clientFactory, logger)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int pageIndex = 1, int pageSize = 10, string searchTerm = null)
        {
            try
            {
                int? spaId = ViewData["SpaId"] != null && ViewData["SpaId"].ToString() != "ALL"
                    ? int.Parse(ViewData["SpaId"].ToString())
                    : (int?)null;

                if (ViewData["SpaId"] == null || ViewData["SpaId"].ToString() == "ALL")
                {
                    TempData["Error"] = "Chọn chi nhánh để xem danh sách giường.";
                    return RedirectToAction("ListRoom", "Room");
                }

                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Room/GetAllRoom");
                HttpResponseMessage response3 = await client.GetAsync($"{apiUrl}/Bed/GetAllBedsPaging?pageIndex={pageIndex}&pageSize={1000}&searchTerm={searchTerm}");
                if (response.IsSuccessStatusCode&&response3.IsSuccessStatusCode)
                {
                    string jsonString = await response.Content.ReadAsStringAsync();
                    var roomList = JsonConvert.DeserializeObject<List<RoomViewModel>>(jsonString);
                    roomList = roomList.Where(x => x.SpaId == spaId).ToList();
                    string jsonString3 = await response3.Content.ReadAsStringAsync();
                    var bedList = JsonConvert.DeserializeObject<PaginatedList<BedViewModelIndex>>(jsonString3);

                    var megreList = new List<BedViewModelIndex>();
                    var totalCount = 0;
                    foreach (var room in roomList)
                    {
                        var filterbed = bedList.Items.Where(x=>x.RoomId==room.Id);
                        // Gán RoomName cho từng bed
                        foreach (var bed in filterbed)
                        {
                            bed.RoomName = room.RoomName;
                            totalCount = totalCount + 1;
                        }

                        megreList = megreList.Concat(filterbed).ToList();
                    }
                    return View(new PaginatedList<BedViewModelIndex>
                    {
                        Items = megreList,
                        TotalCount = totalCount,
                        PageIndex = bedList.PageIndex,
                        PageSize = bedList.PageSize,
                        SearchTerm = bedList.SearchTerm // Assuming both lists have the same SearchTerm
                    }); 
                }
                return View(null); 
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
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

            var cardInvoice = new CardInvoiceDTO();
            int? spaId = ViewData["SpaId"] != null && ViewData["SpaId"].ToString() != "ALL"
                ? int.Parse(ViewData["SpaId"].ToString())
                : (int?)null;
            try
            {
                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();

                if (!ModelState.IsValid)
                {
                    var rooms = await GetAvailableRooms();
                    ViewBag.Rooms = rooms;
                    return View(bedViewModel);
                }
                
                string jsonString = JsonConvert.SerializeObject(bedViewModel);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PostAsync($"{apiUrl}/Bed/AddBed", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

                string errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, errorMessage);
            }
            catch (HttpRequestException httpEx)
            {
                _logger.LogError(httpEx, "Error occurred while making an HTTP request.");
                ModelState.AddModelError(string.Empty, "An error occurred while communicating with the server. Please try again later.");
            }
            catch (JsonSerializationException jsonEx)
            {
                _logger.LogError(jsonEx, "Error occurred while serializing or deserializing JSON.");
                ModelState.AddModelError(string.Empty, "An error occurred with the data format. Please check your inputs.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unexpected error occurred.");
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again.");
            }

            var roomsList = await GetAvailableRooms();
            ViewBag.Rooms = roomsList;
            return View(bedViewModel);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(BedViewModel bedViewModel)
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();
                if (!ModelState.IsValid)
                {
                    var rooms = await GetAvailableRooms();
                    ViewBag.Rooms = rooms;
                    return View(bedViewModel);
                }

                string jsonString = JsonConvert.SerializeObject(bedViewModel);
                var content = new StringContent(jsonString, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync($"{apiUrl}/Bed/UpdateBed/{bedViewModel.Id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("Index");
                }

                string errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, errorMessage);
                ViewBag.ErrorMessage = "Cannot create two beds with the same BedNumber in the same room.";
                var roomsList = await GetAvailableRooms();
                ViewBag.Rooms = roomsList;
                return View(bedViewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            try
            {
                var response = await client.DeleteAsync($"{apiUrl}/api/Bed/DeleteBed/{id}");

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
            var cardInvoice = new CardInvoiceDTO();
            int? spaId = ViewData["SpaId"] != null && ViewData["SpaId"].ToString() != "ALL"
                ? int.Parse(ViewData["SpaId"].ToString())
                : (int?)null;

            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            List<RoomViewModel> rooms = new List<RoomViewModel>();

            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Room/GetAllRoom");

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                rooms = JsonConvert.DeserializeObject<List<RoomViewModel>>(jsonString);
            }

            return rooms.Where(x=>x.SpaId==spaId).ToList();
        }

        private async Task<BedViewModel> GetBedById(int id)
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Bed/GetBedById/{id}");

            if (response.IsSuccessStatusCode)
            {
                string jsonString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<BedViewModel>(jsonString);
            }

            return null;
        }
    }
}
