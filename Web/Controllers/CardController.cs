using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Models;
using System.Text;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json.Linq;
using API.Dtos;
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
        public async Task<IActionResult> ListCard(string? status, int customer, int pageIndex = 1, int pageSize = 10, string searchTerm = null)
        {
            try
            {
                int? spaId = ViewData["SpaId"] != null && ViewData["SpaId"].ToString() != "ALL"
                ? int.Parse(ViewData["SpaId"].ToString())
                : (int?)null;
                ViewData["SelectedStatus"] = status;
                ViewData["SelectedCustomer"] = customer;

                var apiUrl = _configuration["ApiUrl"];
                var url = $"{apiUrl}/Card/GetAll?pageIndex={pageIndex}&pageSize={pageSize}&searchTerm={searchTerm}&spaId={spaId}";
                var client = _clientFactory.CreateClient();
                PaginatedList<CardViewModel> cards = new PaginatedList<CardViewModel>();
                UserDTO user = new UserDTO();
                BranchViewModel branch = new BranchViewModel();
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string data = response.Content.ReadAsStringAsync().Result;
                    cards = JsonConvert.DeserializeObject<PaginatedList<CardViewModel>>(data);

                    foreach (var card in cards.Items)
                    {
                        HttpResponseMessage response1 = await client.GetAsync($"{apiUrl}/users/" + card.CustomerId);
                        string data1 = response1.Content.ReadAsStringAsync().Result;
                        user = JsonConvert.DeserializeObject<UserDTO>(data1);
                        card.CustomerName = user.FirstName + " " + user.MidName + " " + user.LastName;
                        card.CustomerPhone = user.Phone;

                        HttpResponseMessage responseBranch = client.GetAsync($"{apiUrl}/Branch/GetById?id=" + card.BranchId).Result;

                        if (responseBranch.IsSuccessStatusCode)
                        {
                            string responseBranchBody = responseBranch.Content.ReadAsStringAsync().Result;
                            JObject jsonBranch = JObject.Parse(responseBranchBody);
                            card.BranchName = jsonBranch["spaName"].ToString();
                        }
                        else
                        {
                            ViewData["Error"] = "Có lỗi xảy ra";
                        }
                    }

                    var response2 = await client.GetAsync($"{apiUrl}/users/role/5");
                    if (response2.IsSuccessStatusCode)
                    {
                        var users = response2.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>().Result;

                        foreach (var userNew in users)
                        {
                            userNew.FullName = string.Join(" ", userNew.FirstName ?? "", userNew.MidName ?? "", userNew.LastName ?? "").Trim();
                            userNew.FullName = string.Join(", ", userNew.FullName ?? "", userNew.Phone ?? "").Trim();
                        }
                        ViewBag.Users = users;
                    }
                    else
                    {
                        ViewData["Error"] = "Có lỗi xảy ra";
                    }

                    if (spaId != null)
                    {
                        cards.Items = cards.Items.Where(c => c.BranchId == spaId).ToList();
                    }

                    if (!string.IsNullOrEmpty(status))
                    {
                        cards.Items = cards.Items.Where(c => c.Status.Equals(status, StringComparison.OrdinalIgnoreCase)).ToList();
                    }

                    if (!customer.Equals(0))
                    {
                        cards.Items = cards.Items.Where(c => c.CustomerId.Equals(customer)).ToList();
                    }
                }

                return View(cards);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
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
                ComboViewModel combo = new ComboViewModel();
                InvoiceDTO invoice = new InvoiceDTO();
                List<CardComboViewModel> cardCombos = new List<CardComboViewModel>();
                List<CardInvoiceViewModel> cardInvoices = new List<CardInvoiceViewModel>();

                HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Card/GetById?id=" + id);
                HttpResponseMessage responseCombo = await client.GetAsync($"{apiUrl}/Card/GetCardComboByCard?id=" + id);
                HttpResponseMessage responseInvoice = await client.GetAsync($"{apiUrl}/Card/GetCardInvoiceByCard?id=" + id);

                if (TempData.ContainsKey("Error"))
                {
                    ViewData["Error"] = TempData["Error"];
                }

                if (response.IsSuccessStatusCode && responseCombo.IsSuccessStatusCode && responseInvoice.IsSuccessStatusCode)
                {
                    string data = await response.Content.ReadAsStringAsync();
                    card = JsonConvert.DeserializeObject<CardViewModel>(data);

                    string dataCombo = await responseCombo.Content.ReadAsStringAsync();
                    string dataInvoice = await responseInvoice.Content.ReadAsStringAsync();

                    HttpResponseMessage response1 = await client.GetAsync($"{apiUrl}/Branch/GetById?id=" + card.BranchId);
                    string data1 = await response1.Content.ReadAsStringAsync();
                    var branchName = JsonConvert.DeserializeObject<BranchViewModel>(data1).SpaName;
                    ViewBag.BranchName = branchName;

                    cardCombos = JsonConvert.DeserializeObject<List<CardComboViewModel>>(dataCombo);
                    cardInvoices = JsonConvert.DeserializeObject<List<CardInvoiceViewModel>>(dataInvoice);

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
                    }

                    foreach (var ci in cardInvoices)
                    {
                        HttpResponseMessage response4 = await client.GetAsync($"{apiUrl}/Card/GetInvoice?id=" + ci.InvoiceId);
                        string data4 = await response4.Content.ReadAsStringAsync();
                        invoice = JsonConvert.DeserializeObject<InvoiceDTO>(data4);
                        ci.Amount = invoice.Amount;
                        ci.InvoiceDate = invoice.InvoiceDate;
                        ci.Description = invoice.Description;
                        ci.Status = invoice.Status;
                        ci.CustomerName = invoice.CustomerName;
                    }
                }

                if (card == null)
                {
                    ViewData["Error"] = "Không tìm thấy thẻ";
                }
                ViewBag.CardCombos = cardCombos;
                ViewBag.CardInvoices = cardInvoices;
                return View(card);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CreateCard()
        {
            try
            {
                int? spaId = ViewData["SpaId"] != null && ViewData["SpaId"].ToString() != "ALL"
                ? int.Parse(ViewData["SpaId"].ToString())
                : (int?)null;
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
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CreateCard(CardCreateModel card, string selectedCardIds, decimal totalPrice)
        {
            try
            {
                var cardInvoice = new CardInvoiceDTO();
                int? spaId = ViewData["SpaId"] != null && ViewData["SpaId"].ToString() != "ALL"
                    ? int.Parse(ViewData["SpaId"].ToString())
                    : (int?)null;

                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();
                ComboViewModel combo = new ComboViewModel();
                card.Id = 0;
                card.CardNumber = "SenVip" + DateTime.Now.ToString("yyyyMMddHHmmss");
                card.CreateDate = DateTime.Now;
                card.Status = "Active";
                card.BranchId = spaId;

                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, error = "Model validation failed." });
                }

                var json = JsonConvert.SerializeObject(card);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync($"{apiUrl}/Card/Create", content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error creating card: {0}", await response.Content.ReadAsStringAsync());
                    return Json(new { success = false, error = "Khách hàng không tồn tại." });
                }

                if (!string.IsNullOrEmpty(selectedCardIds) && selectedCardIds != "[]")
                {
                    HttpResponseMessage response1 = await client.GetAsync($"{apiUrl}/Card/GetByNumNamePhone?input=" + card.CardNumber);
                    string data = await response1.Content.ReadAsStringAsync();
                    List<CardViewModel> cardCreated = JsonConvert.DeserializeObject<List<CardViewModel>>(data);
                    int idCard = cardCreated.FirstOrDefault()?.Id ?? 0;
                    cardInvoice.CardId = idCard;
                    if (idCard == 0)
                    {
                        return Json(new { success = false, error = "Failed to retrieve created card ID." });
                    }

                    List<int> selectedIds = JsonConvert.DeserializeObject<List<int>>(selectedCardIds);

                    foreach (var id in selectedIds)
                    {
                        HttpResponseMessage responseCombo = await client.GetAsync($"{apiUrl}/Combo/GetByID?IdCombo=" + id);
                        string dataCombo = await responseCombo.Content.ReadAsStringAsync();
                        combo = JsonConvert.DeserializeObject<ComboViewModel>(dataCombo);

                        var cardCombo = new CardComboViewModel
                        {
                            Id = 0,
                            CardId = idCard,
                            ComboId = id,
                            SessionDone = 0,
                            SessionLeft = combo.Quantity
                        };

                        var jsonCardCombo = JsonConvert.SerializeObject(cardCombo);
                        var contentCardCombo = new StringContent(jsonCardCombo, Encoding.UTF8, "application/json");
                        await client.PostAsync($"{apiUrl}/Card/AddCombo", contentCardCombo);

                    }
                }

                List<int> selectedIds2 = JsonConvert.DeserializeObject<List<int>>(selectedCardIds);
                var invoice = new InvoiceViewModel
                {
                    ComboQuantities = selectedIds2
                                .GroupBy(id => id)
                                .ToDictionary(group => group.Key, group => (int?)group.Count()),
                    CustomerId = card.CustomerId,
                    ComboIds = JsonConvert.DeserializeObject<List<int>>(selectedCardIds).Distinct().ToList(),
                    SpaId = spaId,
                    InvoiceDate = DateTime.Now,
                    Status = "Pending",
                    Amount = totalPrice,
                    Description = "Hóa đơn cho thẻ " + card.CardNumber
                };

                var contentInvoice = new StringContent(JsonConvert.SerializeObject(invoice), Encoding.UTF8, "application/json");
                HttpResponseMessage responseInvoice = await client.PostAsync($"{apiUrl}/AddInvoice", contentInvoice);

                if (!responseInvoice.IsSuccessStatusCode)
                {
                    string errorMessage = await responseInvoice.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to create invoice: {0}", errorMessage);
                    return Json(new { success = false, error = $"An error occurred while adding the invoice: {errorMessage}" });
                }

                var invoiceData = await responseInvoice.Content.ReadAsStringAsync();
                var createdInvoice = JsonConvert.DeserializeObject<InvoiceViewModel>(invoiceData);

                if (createdInvoice.Id == 0)
                {
                    return Json(new { success = false, error = "Failed to retrieve the created invoice ID." });
                }

                cardInvoice.Id = 0;
                cardInvoice.InvoiceId = createdInvoice.Id;

                var jsonCardInvoice = JsonConvert.SerializeObject(cardInvoice);
                var contentCardInvoice = new StringContent(jsonCardInvoice, Encoding.UTF8, "application/json");
                await client.PostAsync($"{apiUrl}/Card/AddInvoice", contentCardInvoice);

                var paymentUrl = Url.Action("Payment", "Invoice", new { id = createdInvoice.Id });
                TempData["SuccessMsg"] = "Thêm thành công!";
                return Json(new { success = true, paymentUrl = paymentUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCard(int id, string selectedCardIds)
        {
            try
            {
                int? spaId = ViewData["SpaId"] != null && ViewData["SpaId"].ToString() != "ALL"
                ? int.Parse(ViewData["SpaId"].ToString())
                : (int?)null;

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
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCard(CardCreateModel card, string selectedCardIds, decimal totalPrice)
        {
            try
            {
                var cardInvoice = new CardInvoiceDTO();
                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();
                ComboViewModel combo = new ComboViewModel();

                if (!ModelState.IsValid)
                {
                    return Json(new { success = false, error = "Model validation failed." });
                }

                var json = JsonConvert.SerializeObject(card);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                // Update the card
                HttpResponseMessage response = await client.PutAsync($"{apiUrl}/Card/Update?id=" + card.Id, content);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Error updating card: {0}", await response.Content.ReadAsStringAsync());
                    return Json(new { success = false, error = "An error occurred while updating the card." });
                }

                // Check if there are selected combos to add
                if (!string.IsNullOrEmpty(selectedCardIds) && selectedCardIds != "[]")
                {
                    HttpResponseMessage response3 = await client.GetAsync($"{apiUrl}/Card/GetByNumNamePhone?input=" + card.CardNumber);
                    string data3 = await response3.Content.ReadAsStringAsync();
                    List<CardViewModel> cardUpdated = JsonConvert.DeserializeObject<List<CardViewModel>>(data3);
                    int idCard = cardUpdated.FirstOrDefault()?.Id ?? 0;
                    cardInvoice.CardId = idCard;

                    if (idCard == 0)
                    {
                        return Json(new { success = false, error = "Failed to retrieve updated card ID." });
                    }

                    List<int> selectedIds = JsonConvert.DeserializeObject<List<int>>(selectedCardIds);

                    foreach (var id in selectedIds)
                    {
                        HttpResponseMessage responseCombo = await client.GetAsync($"{apiUrl}/Combo/GetByID?IdCombo=" + id);
                        string dataCombo = await responseCombo.Content.ReadAsStringAsync();
                        combo = JsonConvert.DeserializeObject<ComboViewModel>(dataCombo);

                        var cardCombo = new CardComboViewModel
                        {
                            Id = 0,
                            CardId = idCard,
                            ComboId = id,
                            SessionDone = 0,
                            SessionLeft = combo.Quantity
                        };

                        var json4 = JsonConvert.SerializeObject(cardCombo);
                        var content4 = new StringContent(json4, Encoding.UTF8, "application/json");
                        await client.PostAsync($"{apiUrl}/Card/AddCombo", content4);
                    }
                }

                List<int> selectedIds2 = JsonConvert.DeserializeObject<List<int>>(selectedCardIds);
                var invoice = new InvoiceViewModel
                {
                    ComboQuantities = selectedIds2
                                .GroupBy(id => id)
                                .ToDictionary(group => group.Key, group => (int?)group.Count()),
                    CustomerId = card.CustomerId,
                    ComboIds = JsonConvert.DeserializeObject<List<int>>(selectedCardIds).Distinct().ToList(),
                    SpaId = card.BranchId,
                    InvoiceDate = DateTime.Now,
                    Status = "Pending",
                    Amount = totalPrice,
                    Description = "Hóa đơn cho thẻ cập nhật " + card.CardNumber
                };

                var contentInvoice = new StringContent(JsonConvert.SerializeObject(invoice), Encoding.UTF8, "application/json");
                HttpResponseMessage responseInvoice = await client.PostAsync($"{apiUrl}/AddInvoice", contentInvoice);

                if (!responseInvoice.IsSuccessStatusCode)
                {
                    string errorMessage = await responseInvoice.Content.ReadAsStringAsync();
                    _logger.LogError("Failed to create invoice: {0}", errorMessage);
                    return Json(new { success = false, error = $"An error occurred while adding the invoice: {errorMessage}" });
                }

                var invoiceData = await responseInvoice.Content.ReadAsStringAsync();
                var createdInvoice = JsonConvert.DeserializeObject<InvoiceViewModel>(invoiceData);

                if (createdInvoice.Id == 0)
                {
                    return Json(new { success = false, error = "Failed to retrieve the created invoice ID." });
                }

                cardInvoice.Id = 0;
                cardInvoice.InvoiceId = createdInvoice.Id;

                var jsonCardInvoice = JsonConvert.SerializeObject(cardInvoice);
                var contentCardInvoice = new StringContent(jsonCardInvoice, Encoding.UTF8, "application/json");
                await client.PostAsync($"{apiUrl}/Card/AddInvoice", contentCardInvoice);

                var paymentUrl = Url.Action("Payment", "Invoice", new { id = createdInvoice.Id });

                TempData["SuccessMsg"] = "Cập nhật thành công!";
                return Json(new { success = true, paymentUrl = paymentUrl });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
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
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> UseCard(int id, int cardId)
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();
                var json = JsonConvert.SerializeObject(id);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync($"{apiUrl}/Card/UseCard?id=" + id, content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("DetailCard", new { id = cardId });
                }
                else
                {
                    TempData["Error"] = "Đã sử dụng hết số buổi của combo.";
                    return RedirectToAction("DetailCard", new { id = cardId });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }
    }
}