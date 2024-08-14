using API.Dtos;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text;
using Web.Models;

namespace Web.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public InvoiceController(IConfiguration configuration)
        {
            _configuration = configuration;
            var apiUrl = _configuration.GetValue<string>("ApiUrl");
            _httpClient = new HttpClient { BaseAddress = new Uri(apiUrl) };
        }
        [HttpGet]
        public async Task<IActionResult> InvoiceList()
        {
            List<InvoiceDTO> viewList = new List<InvoiceDTO>();
            

            HttpResponseMessage response = await _httpClient.GetAsync("/api/ListInvoice");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                viewList = JsonConvert.DeserializeObject<List<InvoiceDTO>>(data);
            }
            else
            {
                // Handle error (e.g., log it)
                ModelState.AddModelError(string.Empty, "An error occurred while fetching invoices.");
            }

            

            ViewData["Title"] = "List Invoices";
            return View(viewList);
        }

        private async Task<List<BranchViewModel>> LoadSpasAsync()
        {
            List<BranchViewModel> spas = new List<BranchViewModel>();
            HttpResponseMessage response = await _httpClient.GetAsync("/api/Branch/GetAll");
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
            List<PromotionViewModel> promotions = new List<PromotionViewModel>();
            HttpResponseMessage response = await _httpClient.GetAsync("/api/ListAllPromotion");
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
            List<CardViewModel> cards = new List<CardViewModel>();
            HttpResponseMessage response = await _httpClient.GetAsync("/api/Card/GetAll");
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
            List<ComboViewModel> combos = new List<ComboViewModel>();
            HttpResponseMessage response = await _httpClient.GetAsync("/api/Combo/GetAllCombo");
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
            List<ServiceViewModel> services = new List<ServiceViewModel>();
            HttpResponseMessage response = await _httpClient.GetAsync("/api/Service/GetAllServices");
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
            try
            {
                // Load data asynchronously and handle errors
                var spas = await LoadSpasAsync();
                ViewBag.Spas = spas ?? new List<BranchViewModel>();

              

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
            try
            {
                // Convert ComboIdsString and ServiceIdsString to lists of integers
                invoiceDto.ComboIds = invoiceDto.ComboIdsString?.Split(',').Select(int.Parse).ToList();
                invoiceDto.ServiceIds = invoiceDto.ServiceIdsString?.Split(',').Select(int.Parse).ToList();
               
                

                // Create a JSON content for the HTTP POST request
                var content = new StringContent(JsonConvert.SerializeObject(invoiceDto), Encoding.UTF8, "application/json");

                // Send the POST request to the API
                HttpResponseMessage response = await _httpClient.PostAsync("/api/AddInvoice", content);

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
            var spas = await LoadSpasAsync();
            ViewBag.Spas = spas ?? new List<BranchViewModel>();

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

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Load additional data for dropdowns or select elements
            var spas = await LoadSpasAsync();
            ViewBag.Spas = spas ?? new List<BranchViewModel>();

            var promotions = await LoadPromotionsAsync();
            ViewBag.Promotions = promotions ?? new List<PromotionViewModel>();

           
            var combos = await LoadCombosAsync();
            ViewBag.Combos = combos ?? new List<ComboViewModel>();

            var services = await LoadServicesAsync();
            ViewBag.Services = services ?? new List<ServiceViewModel>();


            // Fetch invoice details from the API
            HttpResponseMessage response = await _httpClient.GetAsync($"/api/DetailInvoiceById?id={id}");


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

                HttpResponseMessage response = await _httpClient.PutAsync($"/api/EditInvoice/{id}", content);
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
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/DeleteProduct/{id}");

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

    }

}
