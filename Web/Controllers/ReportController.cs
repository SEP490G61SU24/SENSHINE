using API.Dtos;
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

        public async Task<IActionResult> ReportRevenue()
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];
                string timeRange = "1year";
                // Construct the API endpoint URL
                var url = $"{apiUrl}/report-summary?period={timeRange}";

                // Fetch the report summary data from the API
                var combinedReport = await GetDataFromApi<CombinedReportDTO>(url);
                if (combinedReport == null) return View("Error");

                // Pass the data to the view
                var viewModel = new CombinedReportViewModel
                {
                    RevenueReports = combinedReport.RevenueReports,
                    InvoiceStatusSummary = combinedReport.InvoiceStatusSummary,
                    ServiceSummaries = combinedReport.ServiceSummaries,
                    ComboSummaries = combinedReport.ComboSummaries
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
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
