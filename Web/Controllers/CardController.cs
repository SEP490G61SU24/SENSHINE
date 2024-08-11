using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Models;
using System.Text;
using API.Models;
using API.Ultils;
using System.Net.Http;
using System.Drawing.Printing;
using Microsoft.AspNetCore.Mvc.Rendering;
using AutoMapper.Configuration.Annotations;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Microsoft.IdentityModel.Tokens;
using API.Dtos;

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
        public async Task<IActionResult> ListCard(string? status)
        {
            List<CardViewModel> cards = new List<CardViewModel>();
            UserDTO user = new UserDTO();
            BranchViewModel branch = new BranchViewModel();
            HttpResponseMessage response = _client.GetAsync(_client.BaseAddress + "/Card/GetAll").Result;

            if (response.IsSuccessStatusCode)
            {
                string data = response.Content.ReadAsStringAsync().Result;
                cards = JsonConvert.DeserializeObject<List<CardViewModel>>(data);
                if (!status.IsNullOrEmpty())
                {
                    cards = cards.Where(c => c.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
                }
            }

            foreach (var card in cards)
            {
                HttpResponseMessage response1 = _client.GetAsync(_client.BaseAddress + "/user/" + card.CustomerId).Result;
                HttpResponseMessage response2 = _client.GetAsync(_client.BaseAddress + "/Branch/GetById?id=" + card.BranchId).Result;
                string data1 = response1.Content.ReadAsStringAsync().Result;
                user = JsonConvert.DeserializeObject<UserDTO>(data1);
                string data2 = response2.Content.ReadAsStringAsync().Result;
                branch = JsonConvert.DeserializeObject<BranchViewModel>(data2);
                card.CustomerName = user.FirstName + " " + user.MidName + " " + user.LastName;
                card.CustomerPhone = user.Phone;
                card.BranchName = branch.SpaName;
            }

            return View(cards);
        }

        [HttpGet]
        public async Task<IActionResult> DetailCard(int id)
        {
            CardViewModel card = new CardViewModel();
            List<CardComboViewModel> cardCombos = new List<CardComboViewModel>();
            ComboViewModel combo = new ComboViewModel();

            HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress + "/Card/GetById?id=" + id);
            HttpResponseMessage response1 = await _client.GetAsync(_client.BaseAddress + "/Card/GetCardComboByCard?id=" + id);

            if (response.IsSuccessStatusCode && response1.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                string data1 = await response1.Content.ReadAsStringAsync();
                card = JsonConvert.DeserializeObject<CardViewModel>(data);
                cardCombos = JsonConvert.DeserializeObject<List<CardComboViewModel>>(data1);

                HttpResponseMessage response2 = _client.GetAsync(_client.BaseAddress + "/user/" + card.CustomerId).Result;
                if (response2.IsSuccessStatusCode)
                {
                    string response2Body = response2.Content.ReadAsStringAsync().Result;
                    JObject json2 = JObject.Parse(response2Body);
                    card.CustomerName = json2["firstName"].ToString() + " " + json2["midName"].ToString() + " " + json2["lastName"].ToString();
                }
                else
                {
                    Console.WriteLine("Error");
                }
                foreach (var cc in cardCombos)
                {
                    HttpResponseMessage response3 = await _client.GetAsync(_client.BaseAddress + "/Combo/GetByID?IdCombo=" + cc.ComboId);
                    string data3 = await response3.Content.ReadAsStringAsync();
                    combo = JsonConvert.DeserializeObject<ComboViewModel>(data3);
                    cc.ComboName = combo.Name;
                    cc.SessionLeft = combo.Quantity - cc.SessionDone;
                }
            }

            if (card == null)
            {
                return NotFound("Không tìm thấy card");
            }
            ViewBag.CardCombos = cardCombos;
            return View(card);
        }

        [HttpGet]
        public IActionResult CreateCard()
        {
            var response = _client.GetAsync($"http://localhost:5297/api/user/byRole/5").Result;
            var response2 = _client.GetAsync($"http://localhost:5297/api/Combo/GetAllCombo").Result;
            if (response.IsSuccessStatusCode && response2.IsSuccessStatusCode)
            {
                var users = response.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>().Result;
                foreach (var user in users)
                {
                    user.FullName = string.Join(" ", user.FirstName ?? "", user.MidName ?? "", user.LastName ?? "").Trim();
                    user.FullName = string.Join(", ", user.FullName ?? "", user.Phone ?? "").Trim();
                }
                ViewBag.Users = new SelectList(users, "Id", "FullName");
                var combos = response2.Content.ReadFromJsonAsync<IEnumerable<ComboCardViewModel>>().Result;
                foreach (var combo in combos)
                {
                    string formattedNumber = string.Format("{0:N0}", combo.SalePrice);
                    combo.SalePriceString = formattedNumber + " VNĐ";
                }
                ViewBag.Combos = combos;

                return View();
            }
            else
            {
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCard(CardCreateModel card, string selectedCardIds)
        {
            card.Id = 0;
            card.CardNumber = "SenVip" + DateTime.Now.ToString("yyyyMMddHHmmss");
            card.CreateDate = DateTime.Now;
            card.Status = "Active";
            card.BranchId = 1;

            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(card);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _client.PostAsync(_client.BaseAddress + "/Card/Create", content);

                if (response.IsSuccessStatusCode)
                {
                    if (string.IsNullOrEmpty(selectedCardIds) || selectedCardIds == "[]")
                    {
                        return RedirectToAction("ListCard");
                    }
                    else
                    {
                        HttpResponseMessage response1 = _client.GetAsync(_client.BaseAddress + "/Card/GetByNumNamePhone?input=" + card.CardNumber).Result;
                        string data = response1.Content.ReadAsStringAsync().Result;
                        List<CardViewModel> cardCreated = JsonConvert.DeserializeObject<List<CardViewModel>>(data);
                        int idd = 0;
                        foreach (var cct in cardCreated)
                        {
                            idd = cct.Id;
                        }
                        CardComboViewModel cardCombo = new CardComboViewModel();
                        List<CardComboViewModel> listCardCombo = new List<CardComboViewModel>();

                        List<int> selectedIds = JsonConvert.DeserializeObject<List<int>>(selectedCardIds);

                        foreach (var id in selectedIds)
                        {
                            cardCombo.Id = 0;
                            cardCombo.CardId = idd;
                            cardCombo.ComboId = id;
                            cardCombo.SessionDone = 0;
                            var json2 = JsonConvert.SerializeObject(cardCombo);
                            Console.WriteLine(json2);
                            var content2 = new StringContent(json2, Encoding.UTF8, "application/json");
                            HttpResponseMessage response2 = await _client.PostAsync(_client.BaseAddress + "/Card/AddCombo", content2);
                        }
                    }
                    return RedirectToAction("ListCard");
                }
                else
                {
                    var response1 = _client.GetAsync($"http://localhost:5297/api/user/byRole/5").Result;
                    var response2 = _client.GetAsync($"http://localhost:5297/api/Combo/GetAllCombo").Result;
                    if (response1.IsSuccessStatusCode && response2.IsSuccessStatusCode)
                    {
                        var users = response1.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>().Result;
                        foreach (var user in users)
                        {
                            user.FullName = string.Join(" ", user.FirstName ?? "", user.MidName ?? "", user.LastName ?? "").Trim();
                            user.FullName = string.Join(", ", user.FullName ?? "", user.Phone ?? "").Trim();
                        }
                        ViewBag.Users = new SelectList(users, "Id", "FullName");
                        var combos = response2.Content.ReadFromJsonAsync<IEnumerable<ComboCardViewModel>>().Result;
                        foreach (var combo in combos)
                        {
                            string formattedNumber = string.Format("{0:N0}", combo.SalePrice);
                            combo.SalePriceString = formattedNumber + " VNĐ";
                        }
                        ViewBag.Combos = combos;
                        ModelState.AddModelError(string.Empty, "Error");
                        return View(card);
                    }
                }
            }

            return View("Error");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCard(int id, string selectedCardIds)
        {
            CardCreateModel card = null;
            HttpResponseMessage response = await _client.GetAsync(_client.BaseAddress + "/Card/GetById?id=" + id);
            var response1 = _client.GetAsync($"http://localhost:5297/api/user/byRole/5").Result;
            var response2 = _client.GetAsync($"http://localhost:5297/api/Combo/GetAllCombo").Result;
            if (response.IsSuccessStatusCode && response1.IsSuccessStatusCode && response2.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                card = JsonConvert.DeserializeObject<CardCreateModel>(data);
                var users = response1.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>().Result;
                foreach (var user in users)
                {
                    user.FullName = string.Join(" ", user.FirstName ?? "", user.MidName ?? "", user.LastName ?? "").Trim();
                    user.FullName = string.Join(", ", user.FullName ?? "", user.Phone ?? "").Trim();
                }
                ViewBag.Users = new SelectList(users, "Id", "FullName");
                var combos = response2.Content.ReadFromJsonAsync<IEnumerable<ComboCardViewModel>>().Result;
                foreach (var combo in combos)
                {
                    string formattedNumber = string.Format("{0:N0}", combo.SalePrice);
                    combo.SalePriceString = formattedNumber + " VNĐ";
                }
                ViewBag.Combos = combos;
            }

            if (card == null)
            {
                return NotFound("Thẻ không tồn tại");
            }

            return View(card);
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCard(CardCreateModel card, string selectedCardIds)
        {
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(card);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                string requestUrl = _client.BaseAddress + "/Card/Update?id=" + card.Id;

                HttpResponseMessage response = await _client.PutAsync(requestUrl, content);

                if (response.IsSuccessStatusCode)
                {
                    if (string.IsNullOrEmpty(selectedCardIds) || selectedCardIds == "[]")
                    {
                        Console.WriteLine("null " + selectedCardIds);
                        return RedirectToAction("ListCard");
                    }
                    else
                    {
                        Console.WriteLine("k null " + selectedCardIds);
                        HttpResponseMessage response1 = _client.GetAsync(_client.BaseAddress + "/Card/GetByNumNamePhone?input=" + card.CardNumber).Result;
                        string data = response1.Content.ReadAsStringAsync().Result;
                        List<CardViewModel> cardUpdated = JsonConvert.DeserializeObject<List<CardViewModel>>(data);
                        int idd = 0;
                        foreach (var cct in cardUpdated)
                        {
                            idd = cct.Id;
                        }
                        CardComboViewModel cardCombo = new CardComboViewModel();
                        List<CardComboViewModel> listCardCombo = new List<CardComboViewModel>();

                        List<int> selectedIds = JsonConvert.DeserializeObject<List<int>>(selectedCardIds);

                        foreach (var id in selectedIds)
                        {
                            cardCombo.Id = 0;
                            cardCombo.CardId = idd;
                            cardCombo.ComboId = id;
                            cardCombo.SessionDone = 0;
                            var json2 = JsonConvert.SerializeObject(cardCombo);
                            Console.WriteLine(json2);
                            var content2 = new StringContent(json2, Encoding.UTF8, "application/json");
                            HttpResponseMessage response2 = await _client.PostAsync(_client.BaseAddress + "/Card/AddCombo", content2);
                        }
                    }
                    return RedirectToAction("ListCard");
                }
                else
                {
                    var response1 = _client.GetAsync($"http://localhost:5297/api/user/byRole/5").Result;
                    var response2 = _client.GetAsync($"http://localhost:5297/api/Combo/GetAllCombo").Result;
                    if (response1.IsSuccessStatusCode && response2.IsSuccessStatusCode)
                    {
                        var users = response1.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>().Result;
                        foreach (var user in users)
                        {
                            user.FullName = string.Join(" ", user.FirstName ?? "", user.MidName ?? "", user.LastName ?? "").Trim();
                            user.FullName = string.Join(", ", user.FullName ?? "", user.Phone ?? "").Trim();
                        }
                        ViewBag.Users = new SelectList(users, "Id", "FullName");
                        var combos = response2.Content.ReadFromJsonAsync<IEnumerable<ComboCardViewModel>>().Result;
                        foreach (var combo in combos)
                        {
                            string formattedNumber = string.Format("{0:N0}", combo.SalePrice);
                            combo.SalePriceString = formattedNumber + " VNĐ";
                        }
                        ViewBag.Combos = combos;
                        ModelState.AddModelError(string.Empty, "Error");
                        return View(card);
                    }
                }
            }

            return View("Error");
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