using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Models;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using Microsoft.IdentityModel.Tokens;
using API.Dtos;
using API.Models;
using API.Ultils;

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
        public async Task<IActionResult> ListCard(string? status, int pageIndex = 1, int pageSize = 10, string searchTerm = null)
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];
                var url = $"{apiUrl}/Card/GetAll?pageIndex={pageIndex}&pageSize={pageSize}&searchTerm={searchTerm}";
                var client = _clientFactory.CreateClient();
                PaginatedList<CardViewModel> cards = new PaginatedList<CardViewModel>();
                UserDTO user = new UserDTO();
                BranchViewModel branch = new BranchViewModel();
                HttpResponseMessage response = await client.GetAsync(url);

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
                    cards = JsonConvert.DeserializeObject<PaginatedList<CardViewModel>>(data);

                    // Convert to a list to apply LINQ filters
                    var filteredCards = cards.Items.Where(c => c.BranchId == spaId).ToList();

                    if (!string.IsNullOrEmpty(status))
                    {
                        filteredCards = filteredCards.Where(c => c.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
                    }

                    // Re-assign filtered cards back to the PaginatedList if necessary
                    cards.Items = filteredCards;
                }

                foreach (var card in cards.Items)
                {
                    HttpResponseMessage response1 = await client.GetAsync($"{apiUrl}/users/" + card.CustomerId);
                    string data1 = response1.Content.ReadAsStringAsync().Result;
                    user = JsonConvert.DeserializeObject<UserDTO>(data1);
                    card.CustomerName = user.FirstName + " " + user.MidName + " " + user.LastName;
                    card.CustomerPhone = user.Phone;
                }

                return View(cards);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");
                ViewData["Error"] = "Có lỗi xảy ra";
                return View();
            }
        }

        [HttpGet]
        public async Task<IActionResult> DetailCard(int id)
        {
            try
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
                    HttpResponseMessage response4 = await client.GetAsync($"{apiUrl}/Branch/GetById?id=" + card.BranchId);
                    string data4 = await response4.Content.ReadAsStringAsync();
                    var branchName = JsonConvert.DeserializeObject<BranchViewModel>(data4).SpaName;
                    ViewBag.BranchName = branchName;

                    cardCombos = JsonConvert.DeserializeObject<List<CardComboViewModel>>(data1);

                    HttpResponseMessage response2 = await client.GetAsync($"{apiUrl}/users/" + card.CustomerId);
                    if (response2.IsSuccessStatusCode)
                    {
                        string response2Body = response2.Content.ReadAsStringAsync().Result;
                        JObject json2 = JObject.Parse(response2Body);
                        card.CustomerName = json2["firstName"].ToString() + " " + json2["midName"].ToString() + " " + json2["lastName"].ToString();
                        card.CustomerPhone = json2["phone"].ToString();
                    }
                    else
                    {
                        ViewData["Error"] = "Có lỗi xảy ra";
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
                    ViewData["Error"] = "Không tìm thấy thẻ";
                }
                ViewBag.CardCombos = cardCombos;
                return View(card);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");
                ViewData["Error"] = "Có lỗi xảy ra";
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateCard()
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
                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();
                var response = await client.GetAsync($"{apiUrl}/users/role/5");
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
                    ViewData["Error"] = "Có lỗi xảy ra";
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");
                ViewData["Error"] = "Có lỗi xảy ra";
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCard(CardCreateModel card, string selectedCardIds)
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
                        ViewData["Error"] = "Có lỗi xảy ra";
                        return View(card);
                    }
                }

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
        public async Task<IActionResult> UpdateCard(int id, string selectedCardIds)
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
                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();
                CardCreateModel card = null;
                HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Card/GetById?id=" + id);
                var response1 = await client.GetAsync($"{apiUrl}/users/role/5");
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
                    ViewData["Error"] = "Thẻ không tồn tại";
                }

                return View(card);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error");
                ViewData["Error"] = "Có lỗi xảy ra";
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCard(CardCreateModel card, string selectedCardIds)
        {
            try
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
                            return RedirectToAction("ListCard");
                        }
                        else
                        {
                            HttpResponseMessage response3 = await client.GetAsync($"{apiUrl}/Card/GetByNumNamePhone?input=" + card.CardNumber);
                            string data3 = response3.Content.ReadAsStringAsync().Result;
                            List<CardViewModel> cardUpdated = JsonConvert.DeserializeObject<List<CardViewModel>>(data3);
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
                                var json4 = JsonConvert.SerializeObject(cardCombo);
                                var content4 = new StringContent(json4, Encoding.UTF8, "application/json");
                                HttpResponseMessage response4 = await client.PostAsync($"{apiUrl}/Card/AddCombo", content4);
                            }
                        }
                        return RedirectToAction("ListCard");
                    }
                    else
                    {
                        ViewData["Error"] = "Có lỗi xảy ra";
                        return View(card);
                    }
                }

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
        public async Task<IActionResult> ChangeStateCard(int id)
        {
            try
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
                    ViewData["Error"] = "Có lỗi xảy ra khi đổi trạng thái.";
                    return BadRequest();
                }
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