using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Models;
using System.Text;
using API.Models;
using API.Ultils;
using System.Net.Http;

namespace Web.Controllers
{
    public class CardController : Controller
    {
        Uri baseAddress = new Uri("http://localhost:5297/api");
        private readonly HttpClient _client;

        public CardController()
        {
            _client = new HttpClient();
            _client.BaseAddress = baseAddress;
        }

        [HttpGet]
        public async Task<IActionResult> ListCard(string searchInput, DateTime? dateFrom, DateTime? dateTo)
        {
            List<CardViewModel> cards = new List<CardViewModel>();
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Card/GetAll").Result;

            if (dateFrom.HasValue && dateTo.HasValue)
            {
                response = _client.GetAsync(_client.BaseAddress + "/Card/SortByDate?dateFrom=" + dateFrom.Value.ToString("yyyy-MM-dd") + "&dateTo=" + dateTo.Value.ToString("yyyy-MM-dd")).Result;
            }

            if (!string.IsNullOrEmpty(searchInput))
            {
                response = _client.GetAsync(_client.BaseAddress + "/Card/GetByNumNamePhone?input=" + searchInput).Result;
            }

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                cards = JsonConvert.DeserializeObject<List<CardViewModel>>(data);
            }

            return View(cards);
        }

        [HttpGet]
        public IActionResult CreateCard()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> CreateCard(CardViewModel card)
        {
            string date = card.CreateDate.Value.ToString("yyyy-MM-dd");
            DateTime date2 = FormatDateTimeUtils.ParseDateTimeLikeSSMS(date);
            card.CreateDate = date2;

            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(card);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + "/Card/Create", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ListCard");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error");
                    return View(card);
                }
            }

            return View(card);
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCard(int id)
        {
            CardViewModel card = null;
            HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress + "/Card/GetById?id=" + id);

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                card = JsonConvert.DeserializeObject<CardViewModel>(data);
            }

            if (card == null)
            {
                return NotFound("Thẻ không tồn tại");
            }

            return View(card);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCard(CardViewModel card)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(card);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _client.PutAsync(_client.BaseAddress + "/Card/Update?id=" + card.Id, content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ListCard");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Có lỗi xảy ra khi cập nhật thẻ");
                    return View(card);
                }
            }

            return View(card);
        }

        [HttpGet]
        public async Task<IActionResult> ChangeStateCard(int id)
        {
            var json = JsonConvert.SerializeObject(id);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _client.PutAsync(_client.BaseAddress + "/Card/ActiveDeactive?id=" + id, content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListCard");
            }
            else
            {
                return BadRequest("Có lỗi xảy ra khi xóa dịch vụ.");
            }
        }
    }
}