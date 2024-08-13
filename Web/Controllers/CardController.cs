﻿using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Models;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using Microsoft.IdentityModel.Tokens;
using API.Dtos;

namespace Web.Controllers
{
    public class CardController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<UserController> _logger;

        public CardController(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<UserController> logger)
             : base(configuration, clientFactory, logger)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> ListCard(string? status)
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            List<CardViewModel> cards = new List<CardViewModel>();
            UserDTO user = new UserDTO();
            BranchViewModel branch = new BranchViewModel();
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Card/GetAll");
            int? spaId = 1;
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
                cards = JsonConvert.DeserializeObject<List<CardViewModel>>(data);
                cards = cards.Where(c => c.BranchId == spaId).ToList();

                if (!status.IsNullOrEmpty())
                {
                    cards = cards.Where(c => c.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
                }
            }

            foreach (var card in cards)
            {
                HttpResponseMessage response1 = await client.GetAsync($"{apiUrl}/user/" + card.CustomerId);
                string data1 = response1.Content.ReadAsStringAsync().Result;
                user = JsonConvert.DeserializeObject<UserDTO>(data1);
                card.CustomerName = user.FirstName + " " + user.MidName + " " + user.LastName;
                card.CustomerPhone = user.Phone;
            }

            return View(cards);
        }

        [HttpGet]
        public async Task<IActionResult> DetailCard(int id)
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            CardViewModel card = new CardViewModel();
            List<CardComboViewModel> cardCombos = new List<CardComboViewModel>();
            ComboViewModel combo = new ComboViewModel();

            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Card/GetById?id=" + id);
            HttpResponseMessage response1 = await client.GetAsync($"{apiUrl}/Card/GetCardComboByCard?id=" + id);

            if (response.IsSuccessStatusCode && response1.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                string data1 = await response1.Content.ReadAsStringAsync();
                card = JsonConvert.DeserializeObject<CardViewModel>(data);
                cardCombos = JsonConvert.DeserializeObject<List<CardComboViewModel>>(data1);

                HttpResponseMessage response2 = await client.GetAsync($"{apiUrl}/user/" + card.CustomerId);
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
                    HttpResponseMessage response3 = await client.GetAsync($"{apiUrl}/Combo/GetByID?IdCombo=" + cc.ComboId);
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
        public async Task<IActionResult> CreateCard()
        {
            int? spaId = 1;
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
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            var response = await client.GetAsync($"{apiUrl}/user/byRole/5");
            var response2 = await client.GetAsync($"{apiUrl}/Combo/GetAllCombo");
            if (response.IsSuccessStatusCode && response2.IsSuccessStatusCode)
            {
                var users = response.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>().Result;
                users = users.Where(u => u.SpaId == spaId).ToList();
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
                    combo.SalePriceString = formattedNumber + " VND";
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
            int? spaId = 1;
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
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            card.Id = 0;
            card.CardNumber = "SenVip" + DateTime.Now.ToString("yyyyMMddHHmmss");
            card.CreateDate = DateTime.Now;
            card.Status = "Active";
            card.BranchId = spaId;

            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(card);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync($"{apiUrl}/Card/Create", content);

                if (response.IsSuccessStatusCode)
                {
                    if (string.IsNullOrEmpty(selectedCardIds) || selectedCardIds == "[]")
                    {
                        return RedirectToAction("ListCard");
                    }
                    else
                    {
                        HttpResponseMessage response1 = await client.GetAsync($"{apiUrl}/Card/GetByNumNamePhone?input=" + card.CardNumber);
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
                            HttpResponseMessage response2 = await client.PostAsync($"{apiUrl}/Card/AddCombo", content2);
                        }
                    }
                    return RedirectToAction("ListCard");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Error");
                    return View(card);
                }
            }

            return View("Error");
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCard(int id, string selectedCardIds)
        {
            int? spaId = 1;
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
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            CardCreateModel card = null;
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Card/GetById?id=" + id);
            var response1 = await client.GetAsync($"{apiUrl}/user/byRole/5");
            var response2 = await client.GetAsync($"{apiUrl}/Combo/GetAllCombo");
            if (response.IsSuccessStatusCode && response1.IsSuccessStatusCode && response2.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                card = JsonConvert.DeserializeObject<CardCreateModel>(data);
                var users = response1.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>().Result;
                users = users.Where(u => u.SpaId == spaId).ToList();
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
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            if (ModelState.IsValid)
            {
                var json = JsonConvert.SerializeObject(card);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync($"{apiUrl}/Card/Update?id=" + card.Id, content);

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
                        HttpResponseMessage response1 = await client.GetAsync($"{apiUrl}/Card/GetByNumNamePhone?input=" + card.CardNumber);
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
                            HttpResponseMessage response2 = await client.PostAsync($"{apiUrl}/Card/AddCombo", content2);
                        }
                    }
                    return RedirectToAction("ListCard");
                }
                else
                {
                    var response1 = await client.GetAsync($"{apiUrl}/user/byRole/5");
                    var response2 = await client.GetAsync($"{apiUrl}/Combo/GetAllCombo");
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
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            var json = JsonConvert.SerializeObject(id);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PutAsync($"{apiUrl}/Card/ActiveDeactive?id=" + id, content);

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