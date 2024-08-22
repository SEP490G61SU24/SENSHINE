using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Controllers
{
    public class DashboardController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<UserController> _logger;


        public DashboardController(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<UserController> logger) : base(configuration, clientFactory, logger)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            DashboardViewModel model = await GetDashboardDataAsync();
            return View(model);
        }

        private async Task<DashboardViewModel> GetDashboardDataAsync()
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            var model = new DashboardViewModel();

            // Fetch invoice data
            var invoiceResponse = await client.GetAsync($"{apiUrl}/daily-revenue");
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
            var salaryResponse = await client.GetAsync($"{apiUrl}/Salary/GetMonthlySalariesForYear?year={year}");
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
