using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Net.Http;
using Web.Models;

namespace Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            var apiUrl = _configuration.GetValue<string>("ApiUrl");
            _httpClient = new HttpClient { BaseAddress = new Uri(apiUrl) };
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public async Task<IActionResult> HomePageAsync()
        {
            List<NewsViewModel> viewList = new List<NewsViewModel>();
            HttpResponseMessage response = await _httpClient.GetAsync("/api/ListNewsSortDESCByNewDate");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                viewList = JsonConvert.DeserializeObject<List<NewsViewModel>>(data);
            }
            else
            {
                // Log error message here
                ModelState.AddModelError(string.Empty, "An error occurred while fetching the news list.");
            }

            
            return View(viewList);
        }
        public async Task<IActionResult> NewsPage()
        {
            List<NewsViewModel> viewList = new List<NewsViewModel>();
            HttpResponseMessage response = await _httpClient.GetAsync("/api/ListAllNews");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                viewList = JsonConvert.DeserializeObject<List<NewsViewModel>>(data);
            }
            else
            {
                // Log error message here
                ModelState.AddModelError(string.Empty, "An error occurred while fetching the news list.");
            }


            return View(viewList);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        public IActionResult OurService()
        {
            return View();
        }

        public IActionResult AboutUs()
        {
            return View();
        }
    }
}
