using API.Dtos;
using API.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text;
using Web.Models;
namespace Web.Controllers
{
    public class NewsController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public NewsController(IConfiguration configuration)
        {
            _configuration = configuration;
            var apiUrl = _configuration.GetValue<string>("ApiUrl");
            _httpClient = new HttpClient { BaseAddress = new Uri(apiUrl) };
        }
        [HttpGet]
        public async Task<IActionResult> NewsList()
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

            ViewData["Title"] = "List News";
            return View(viewList);
        }

        [HttpGet]
        public IActionResult AddNews()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> AddNews(NewsDTO newsDto)
        {
            if (!ModelState.IsValid)
            {
                return View(newsDto);
            }

            string json = JsonConvert.SerializeObject(newsDto);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync("/api/AddNews", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("NewsList");
            }

            // Log error message here
            ModelState.AddModelError(string.Empty, "An error occurred while adding the news.");
            return View(newsDto);
        }

        [HttpGet]
        public async Task<IActionResult> EditNews(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/api/GetNewsDetail/{id}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var news = JsonConvert.DeserializeObject<NewsDTO>(data);
                return View(news);
            }

            return NotFound();
        }

        [HttpPost]
        public async Task<IActionResult> EditNews(int id, NewsDTO newsDto)
        {
            if (!ModelState.IsValid)
            {
                return View(newsDto);
            }

            string json = JsonConvert.SerializeObject(newsDto);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync($"/api/EditNews/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("NewsList");
            }

            // Log error message here
            ModelState.AddModelError(string.Empty, "An error occurred while editing the news.");
            return View(newsDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetNewsDetail(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/api/GetNewsDetail/{id}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var news = JsonConvert.DeserializeObject<NewsDTO>(data);
                return View(news);
            }

            return NotFound();
        }

        [HttpGet]
        public async Task<IActionResult> NewsByDate(DateTime from, DateTime to)
        {
            string fromDateString = from.ToString("s");
            string toDateString = to.ToString("s");

            HttpResponseMessage response = await _httpClient.GetAsync($"/api/NewsByDate?from={fromDateString}&to={toDateString}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var newsList = JsonConvert.DeserializeObject<IEnumerable<NewsDTORequest>>(data);
                return View(newsList);
            }

            return View(new List<NewsDTORequest>());
        }

        [HttpPost]
        public async Task<IActionResult> DeleteNews(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"/api/DeleteNews/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("NewsList");
            }

            return NotFound();
        }
    }
}