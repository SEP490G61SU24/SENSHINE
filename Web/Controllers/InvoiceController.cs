using API.Dtos;
using API.Models;
using API.Ultils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using Web.Models;



namespace Web.Controllers
{
    public class InvoiceController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<UserController> _logger;

        public InvoiceController(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<UserController> logger) : base(configuration, clientFactory, logger)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
        }
        private async Task<UserViewModel> LoadUserAsync()
        {
            var user = new UserViewModel();
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                var userProfile = await GetUserProfileAsync(token);

                if (userProfile != null)
                {
                    user.Id = userProfile.Id;
                    user.UserName = userProfile.UserName;
                    user.FirstName = userProfile.FirstName;
                    user.MidName = userProfile.MidName;
                    user.LastName = userProfile.LastName;
                    user.Phone = userProfile.Phone;
                    user.BirthDate = userProfile.BirthDate;
                    user.Status = userProfile.Status;
                    user.StatusWorking = userProfile.StatusWorking;
                    user.SpaId = userProfile.SpaId;
                    user.ProvinceCode = userProfile.ProvinceCode;
                    user.DistrictCode = userProfile.DistrictCode;
                    user.WardCode = userProfile.WardCode;
                    user.Address = userProfile.Address;
                    user.Roles = userProfile.Roles;
                    user.RoleName = userProfile.RoleName;
                    user.RoleId = userProfile.RoleId;
                    user.FullName = $"{userProfile.FirstName} {userProfile.MidName} {userProfile.LastName}";
                }
                else
                {
                    ViewData["Error"] = "Failed to retrieve user profile.";
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An error occurred while fetching the user profile.");
            }

            return user;
        }
        [HttpGet]
        public async Task<IActionResult> InvoiceList(int? idspa, int pageIndex = 1, int pageSize = 10, string searchTerm = null, DateTime? startDate = null, DateTime? endDate = null,string? status= null)
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            var use = await LoadUserAsync();
            idspa =  use.SpaId;
            var urlBuilder = new StringBuilder($"{apiUrl}/GetInvoicesPaging?");

            if (idspa != null)
            {
                urlBuilder.Append($"idspa={idspa}&");
            }

            if (startDate != null)
            {
                urlBuilder.Append($"startDate={startDate}&");
            }

            if (endDate != null)
            {
                urlBuilder.Append($"endDate={endDate}&");
            }
            if (status != null)
            {
                urlBuilder.Append($"status={status}&");
            }
            urlBuilder.Append($"pageIndex={pageIndex}&pageSize={pageSize}&searchTerm={searchTerm}");

            // Remove the trailing '&' if it exists
            var url = urlBuilder.ToString().TrimEnd('&');

            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                var paginatedResult = await response.Content.ReadFromJsonAsync<FilteredPaginatedList<InvoiceDTO>>();
                paginatedResult.SearchTerm = searchTerm;
                return View(paginatedResult);
            }
            else
            {
                return View("Error");
            }
        }
       

        private async Task<List<BranchViewModel>> LoadSpasAsync()
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            List<BranchViewModel> spas = new List<BranchViewModel>();
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Branch/GetAll");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                spas = JsonConvert.DeserializeObject<List<BranchViewModel>>(data);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An error occurred while fetching spas.");
            }
            return spas;
        }



        private async Task<List<PromotionViewModel>> LoadPromotionsAsync()
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            List<PromotionViewModel> promotions = new List<PromotionViewModel>();
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/ListAllPromotion");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                promotions = JsonConvert.DeserializeObject<List<PromotionViewModel>>(data);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An error occurred while fetching promotions.");
            }
            return promotions;
        }

        private async Task<List<CardViewModel>> LoadCardsAsync()
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            List<CardViewModel> cards = new List<CardViewModel>();
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Card/GetAll");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                cards = JsonConvert.DeserializeObject<List<CardViewModel>>(data);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An error occurred while fetching cards.");
            }
            return cards;
        }

        private async Task<List<ComboViewModel>> LoadCombosAsync()
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            List<ComboViewModel> combos = new List<ComboViewModel>();
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Combo/GetAllCombo");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                combos = JsonConvert.DeserializeObject<List<ComboViewModel>>(data);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An error occurred while fetching combos.");
            }
            return combos;
        }

        private async Task<List<ServiceViewModel>> LoadServicesAsync()
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            List<ServiceViewModel> services = new List<ServiceViewModel>();
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Service/GetAllServices");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                services = JsonConvert.DeserializeObject<List<ServiceViewModel>>(data);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An error occurred while fetching services.");
            }
            return services;
        }


        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            try
            {
               

                var promotions = await LoadPromotionsAsync();
                ViewBag.Promotions = promotions ?? new List<PromotionViewModel>();

                var combos = await LoadCombosAsync();
                ViewBag.Combos = combos ?? new List<ComboViewModel>();

                var services = await LoadServicesAsync();
                ViewBag.Services = services ?? new List<ServiceViewModel>();
            }
            catch (Exception ex)
            {

                ModelState.AddModelError(string.Empty, "An error occurred while loading data.");
            }

            return View();
        }


        [HttpPost]
        public async Task<IActionResult> Add(InvoiceViewModel invoiceDto)
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            var use = await LoadUserAsync();
            try
            {
                // Convert ComboIdsString and ServiceIdsString to lists of integers
                invoiceDto.ComboIds = invoiceDto.ComboIdsString?.Split(',').Select(int.Parse).ToList();
                invoiceDto.ServiceIds = invoiceDto.ServiceIdsString?.Split(',').Select(int.Parse).ToList();
                invoiceDto.SpaId= use.SpaId ;


                // Create a JSON content for the HTTP POST request
                var content = new StringContent(JsonConvert.SerializeObject(invoiceDto), Encoding.UTF8, "application/json");

                // Send the POST request to the API
                HttpResponseMessage response = await client.PostAsync($"{apiUrl}/AddInvoice", content);

                if (response.IsSuccessStatusCode)
                {
                    // Redirect to InvoiceList if the request is successful
                    return RedirectToAction("InvoiceList");
                }

                // Read the error message from the response
                string errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"An error occurred while adding the invoice: {errorMessage}");
            }
            catch (Exception ex)
            {
                // Log the exception (e.g., _logger.LogError(ex, "Failed to add invoice"))
                ModelState.AddModelError(string.Empty, "An error occurred while adding the invoice.");
            }

            // Load necessary data for the view if an error occurred
            //var spas = await LoadSpasAsync();
            //ViewBag.Spas = spas ?? new List<BranchViewModel>();

            var promotions = await LoadPromotionsAsync();
            ViewBag.Promotions = promotions ?? new List<PromotionViewModel>();

            var cards = await LoadCardsAsync();
            ViewBag.Cards = cards ?? new List<CardViewModel>();

            var combos = await LoadCombosAsync();
            ViewBag.Combos = combos ?? new List<ComboViewModel>();

            var services = await LoadServicesAsync();
            ViewBag.Services = services ?? new List<ServiceViewModel>();

            // Return the view with the invoiceDto if there was an error
            return View(invoiceDto);
        }


        /* private async Task<List<ComboViewModel>> LoadCombosByInvoiceIdAsync(int Id)
         {
             List<ComboViewModel> combos = new List<ComboViewModel>();
             HttpResponseMessage comboResponse = await _httpClient.GetAsync($"api/GetCombosByInvoiceId?id={Id}");

             if (comboResponse.IsSuccessStatusCode)
             {
                 string responseData = await comboResponse.Content.ReadAsStringAsync();

                 combos = JsonConvert.DeserializeObject<List<ComboViewModel>>(responseData);
             }
             else
             {
                 // Handle error (e.g., log it)
                 ModelState.AddModelError(string.Empty, "An error occurred while fetching combos.");
             }

             return combos;
         }
         private async Task<List<ServiceViewModel>> LoadServicesByInvoiceIdAsync(int Id)
         {
             List<ServiceViewModel> services = new List<ServiceViewModel>();
             HttpResponseMessage serviceResponse = await _httpClient.GetAsync($"/GetServicesByInvoiceId?id={Id}");

             if (serviceResponse.IsSuccessStatusCode)
             {
                 string responseData = await serviceResponse.Content.ReadAsStringAsync();

                 services = JsonConvert.DeserializeObject<List<ServiceViewModel>>(responseData);
             }
             else
             {
                 // Handle error (e.g., log it)
                 ModelState.AddModelError(string.Empty, "An error occurred while fetching services.");
             }

             return services;
         }*/
        public static string GenerateQRCodeUrl(string? content, string price)
        {
            string encodedContent = Uri.EscapeDataString(content ?? string.Empty);
            string encodedPrice = Uri.EscapeDataString(price);
            return $"https://img.vietqr.io/image/{MyBank.Bank.BankId}-{MyBank.Bank.AccountNo}-compact2.png?amount={encodedPrice}&addInfo={encodedContent}";
        }

        [HttpGet]
        public async Task<IActionResult> Payment(int id)
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            // Fetch invoice details from the API
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/DetailInvoiceById?id={id}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var invoiceDto = JsonConvert.DeserializeObject<InvoiceViewModel>(data);
               

                if (invoiceDto == null)
                {
                    ModelState.AddModelError(string.Empty, "Invoice not found.");
                    return RedirectToAction("InvoiceList");
                }
                
                // Generate QR code URL for payment
                var qrCodeUrl = GenerateQRCodeUrl(invoiceDto.CustomerName + " chuyển khoản cho SenShineSPa.", invoiceDto.Amount.ToString());
                ViewBag.QRCodeUrl = qrCodeUrl;
                return View(invoiceDto);
            }

            // Handle failure to fetch invoice details
            ModelState.AddModelError(string.Empty, "Error fetching invoice details.");
            return RedirectToAction("InvoiceList");
        }






        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            // Load additional data for dropdowns or select elements
           /* var spas = await LoadSpasAsync();
            ViewBag.Spas = spas ?? new List<BranchViewModel>();*/

            var promotions = await LoadPromotionsAsync();
            ViewBag.Promotions = promotions ?? new List<PromotionViewModel>();


            var combos = await LoadCombosAsync();
            ViewBag.Combos = combos ?? new List<ComboViewModel>();

            var services = await LoadServicesAsync();
            ViewBag.Services = services ?? new List<ServiceViewModel>();


            // Fetch invoice details from the API
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/DetailInvoiceById?id={id}");


            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var invoiceDto = JsonConvert.DeserializeObject<InvoiceViewModel>(data);

                if (invoiceDto == null)
                {
                    ModelState.AddModelError(string.Empty, "Invoice not found.");
                    return RedirectToAction("InvoiceList");
                }

                return View(invoiceDto);
            }

            // Handle failure to fetch invoice details
            ModelState.AddModelError(string.Empty, "Error fetching invoice details.");
            return RedirectToAction("InvoiceList");
        }



        // POST: InvoiceController/Edit/5
        [HttpPost]

        public async Task<IActionResult> Edit(int id, InvoiceViewModel invoiceDto)

        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            var spas = await LoadSpasAsync();
            ViewBag.Spas = spas ?? new List<BranchViewModel>();

            var promotions = await LoadPromotionsAsync();
            ViewBag.Promotions = promotions ?? new List<PromotionViewModel>();



            var combos = await LoadCombosAsync();
            ViewBag.Combos = combos ?? new List<ComboViewModel>();

            var services = await LoadServicesAsync();
            ViewBag.Services = services ?? new List<ServiceViewModel>();


            try
            {
                invoiceDto.ComboIds = invoiceDto.ComboIdsString?.Split(',').Select(int.Parse).ToList();
                invoiceDto.ServiceIds = invoiceDto.ServiceIdsString?.Split(',').Select(int.Parse).ToList();
                var selectedPromotion = promotions?.FirstOrDefault(p => p.PromotionName.Equals(invoiceDto.PromotionName));
                invoiceDto.PromotionId = selectedPromotion?.Id;
                var selectedSpa = spas?.FirstOrDefault(s => s.SpaName.Equals(invoiceDto.SpaName));
                invoiceDto.SpaId = selectedSpa?.Id;


                string jsonData = JsonConvert.SerializeObject(invoiceDto);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await client.PutAsync($"{apiUrl}/EditInvoice/{id}", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(InvoiceList));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while updating the invoice.");
                }
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "An exception occurred while updating the invoice.");
            }

            return View(invoiceDto);
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            try
            {
                var response = await client.DeleteAsync($"{apiUrl}/DeleteProduct/{id}");

                if (response.IsSuccessStatusCode)
                {

                    return Json(new { success = true });
                }


                return Json(new { success = false, message = "An error occurred while deleting the product." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An unexpected error occurred." });
            }
        }
        [HttpGet]
        public async Task<IActionResult> CheckPayment(string content, decimal price)
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            try
            {
                // Define the URL for the API endpoint
                string url = "https://script.google.com/macros/s/AKfycby3FhAxN37cH06MpxEn-3x1foUnnX_Q70w0fC3A6BeHqJEQIOiPPrJjDQgG2XHL6-Hm/exec";

                // Send a GET request to the API endpoint
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    // Read and deserialize the JSON response
                    string data = await response.Content.ReadAsStringAsync();
                    var paymentResponse = JsonConvert.DeserializeObject<PaymentResponse>(data);

                    if (paymentResponse == null || paymentResponse.Data == null || paymentResponse.Error)
                    {
                        return BadRequest(new { message = "Invalid response data" });
                    }

                    // Find the most recent valid record based on content and price
                    var validRecord = paymentResponse.Data
                        .Where(record => record.Content.Contains(content) && record.Price == price)
                        .OrderByDescending(record => record.CreateAt)
                        .FirstOrDefault();

                    if (validRecord != null)
                    {
                       
                        return Ok(new { success = true, record = validRecord });
                    }
                    else
                    {
                        // No valid record found
                        return NotFound(new { success = false, message = "No valid record found" });
                    }
                }
                else
                {
                    // Handle unsuccessful status code
                    return StatusCode((int)response.StatusCode, new { message = "Failed to retrieve data" });
                }
            }
            catch (HttpRequestException httpEx)
            {
                // Handle HTTP request errors
                return StatusCode(500, new { message = $"HTTP Request Error: {httpEx.Message}" });
            }
            catch (JsonSerializationException jsonEx)
            {
                // Handle JSON deserialization errors
                return StatusCode(500, new { message = $"JSON Deserialization Error: {jsonEx.Message}" });
            }
            catch (Exception ex)
            {
                // Handle any other errors
                return StatusCode(500, new { message = $"General Error: {ex.Message}" });
            }
        }
       

    }

}
