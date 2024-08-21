using API.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Web.Models;

namespace Web.Controllers
{
    public class ReportController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<ReportController> _logger;

        public ReportController(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<ReportController> logger)
            : base(configuration, clientFactory, logger)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
        }

        // GET: ReportController/ReportRevenue
        public async Task<IActionResult> ReportRevenue()
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();

            // Get the current year
            var now = DateTime.Now;
            var startDate = new DateTime(now.Year, 1, 1); // Start of the year
            var endDate = new DateTime(now.Year, 12, 31); // End of the year

            // Fetch revenue report data
            var revenueReports = await GetDataFromApi<List<RevenueReport>>($"{apiUrl}/revenue-report?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}");
            if (revenueReports == null) return View("Error");

            // Fetch invoice status summary data
            var invoiceStatusSummary = await GetDataFromApi<List<RevenueReport>>($"{apiUrl}/invoice-status-summary");
            if (invoiceStatusSummary == null) return View("Error");

            // Fetch service summary data
            var serviceSummary = await GetDataFromApi<List<ServiceSummary>>($"{apiUrl}/invoice-service-summary");
            if (serviceSummary == null) return View("Error");

            // Fetch combo summary data
            var comboSummary = await GetDataFromApi<List<ComboSummary>>($"{apiUrl}/invoice-combo-summary");
            if (comboSummary == null) return View("Error");

            // Combine data into view model
            var combinedReport = new CombinedReportViewModel
            {
                RevenueReports = revenueReports,
                InvoiceStatusSummary = invoiceStatusSummary,
                ComboSummaries = comboSummary,
                ServiceSummaries = serviceSummary
            };

            // Pass combined data to the view
            return View(combinedReport);
        }

        private async Task<T> GetDataFromApi<T>(string url)
        {
            var client = _clientFactory.CreateClient();
            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(data);
            }
            else
            {
                _logger.LogError($"Failed to fetch data from {url}. Status code: {response.StatusCode}");
                ModelState.AddModelError(string.Empty, "An error occurred while fetching data.");
                return default;
            }
        }
    }
}
