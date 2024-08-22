using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public DashboardController(IConfiguration configuration)
        {
            _configuration = configuration;
            var apiUrl = _configuration.GetValue<string>("ApiUrl");
            _httpClient = new HttpClient { BaseAddress = new Uri(apiUrl) };
        }

        public async Task<IActionResult> Index()
        {
            DashboardViewModel model = await GetDashboardDataAsync();
            return View(model);
        }

        private async Task<DashboardViewModel> GetDashboardDataAsync()
        {
            var model = new DashboardViewModel();

            // Fetch invoice data
            var invoiceResponse = await _httpClient.GetAsync("/api/daily-revenue");
            if (invoiceResponse.IsSuccessStatusCode)
            {
                string invoiceData = await invoiceResponse.Content.ReadAsStringAsync();
                var invoiceResult = JsonConvert.DeserializeObject<DashboardViewModel>(invoiceData);
                model.Labels = invoiceResult.Labels;
                model.Values = invoiceResult.Values;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An error occurred while fetching invoice data.");
            }

            // Fetch salary data with the year parameter
            int year = 2024; // Default year
            var salaryResponse = await _httpClient.GetAsync($"/api/Salary/GetMonthlySalariesForYear?year={year}");
            if (salaryResponse.IsSuccessStatusCode)
            {
                string salaryData = await salaryResponse.Content.ReadAsStringAsync();
                var salaryResult = JsonConvert.DeserializeObject<DashboardViewModel>(salaryData);
                model.SalaryMonths = salaryResult.SalaryMonths;
                model.TotalSalaries = salaryResult.TotalSalaries;
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An error occurred while fetching salary data.");
            }

            return model;
        }
    }
}
