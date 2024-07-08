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
        Uri baseAddress = new Uri("http://localhost:5297/api");
        private readonly HttpClient _httpClient;
        public NewsController()
        {
            _httpClient = new HttpClient();
            _httpClient.BaseAddress = baseAddress;
        }
        [HttpGet]
        public async Task<IActionResult> NewsList()
        {
            List<NewsViewModel> viewList = new List<NewsViewModel>();
            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + "/ListAllNews");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                viewList = JsonConvert.DeserializeObject<List<NewsViewModel>>(data);
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
            string json = JsonConvert.SerializeObject(newsDto);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await _httpClient.PostAsync(_httpClient.BaseAddress + "/AddNews", content);

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var news = JsonConvert.DeserializeObject<News>(data);
                return RedirectToAction("NewsList");
            }

            return View(newsDto);
        }
        [HttpGet]
        public async Task<IActionResult> EditNews(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/GetNewsDetail/{id}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var news = JsonConvert.DeserializeObject<NewsDTO>(data);
                return View(news);
            }

            return NotFound();
        }

        [HttpPut("EditNews/{id}")]
        public async Task<IActionResult> EditNews(int id, [FromBody] NewsDTO newsDto)
        {
            string json = JsonConvert.SerializeObject(newsDto);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await _httpClient.PutAsync($"EditNews/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var news = JsonConvert.DeserializeObject<News>(data);
                return RedirectToAction("NewsList");
            }

            
            return View(newsDto);
        }

        [HttpGet]
        public async Task<IActionResult> GetNewsDetail(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + $"/GetNewsDetail/{id}");

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
            HttpResponseMessage response = await _httpClient.GetAsync(_httpClient.BaseAddress + $"/NewsByDate?from={from.ToString("s")}&to={to.ToString("s")}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var newsList = JsonConvert.DeserializeObject<IEnumerable<NewsDTORequest>>(data);
                return View(newsList);
            }

            return View(new List<NewsDTORequest>());
        }

        [HttpDelete]
        public async Task<IActionResult> DeleteNews(int id)
        {
            HttpResponseMessage response = await _httpClient.DeleteAsync($"DeleteNews/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("NewsList");
            }

            return NotFound();
        }

    }
}
