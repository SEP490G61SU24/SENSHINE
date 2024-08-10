using API.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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

                var cards = await LoadCardsAsync();
                ViewBag.Cards = cards ?? new List<CardViewModel>();

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
        
        public async Task<IActionResult> Add(InvoiceDTO invoiceDto)
        {
            

            try
            {
                string jsonData = JsonConvert.SerializeObject(invoiceDto);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync("/api/AddInvoice", content);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(InvoiceList));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "An error occurred while creating the invoice.");
                }
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "An exception occurred while creating the invoice.");
            }

            return View(invoiceDto);
        }

        // GET: InvoiceController/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            InvoiceDTO invoice = null;

            HttpResponseMessage response = await _httpClient.GetAsync($"/api/GetInvoice/{id}");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                invoice = JsonConvert.DeserializeObject<InvoiceDTO>(data);
            }
            else
            {
                return NotFound();
            }

            return View(invoice);
        }

        // POST: InvoiceController/Edit/5
        [HttpPost]
        
        public async Task<IActionResult> Edit(int id, InvoiceDTO invoiceDto)
        {
            if (!ModelState.IsValid)
            {
                return View(invoiceDto);
            }

            try
            {
                string jsonData = JsonConvert.SerializeObject(invoiceDto);
                StringContent content = new StringContent(jsonData, Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PutAsync($"/api/UpdateInvoice/{id}", content);
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
