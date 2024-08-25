using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using System.Diagnostics;
using Web.Models;

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

    [Route("/")]
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

    [Route("Home/Public/[action]")]
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

    [Route("Home/Public/[action]/{id}")]
    public async Task<IActionResult> NewsDetail(int id)
    {
        HttpResponseMessage response = await _httpClient.GetAsync($"api/GetNewsDetail/{id}");

        if (response.IsSuccessStatusCode)
        {
            var newsDetail = await response.Content.ReadFromJsonAsync<NewsViewModel>();
            return View(newsDetail);
        }

        return View("Error");
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }

    [Route("Home/Public/[action]")]
    public IActionResult OurService()
    {
        return View();
    }

    [Route("Home/Public/[action]")]
    public IActionResult AboutUs()
    {
        return View();
    }
}
