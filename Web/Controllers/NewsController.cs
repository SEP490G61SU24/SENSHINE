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
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(NewsDTO newsDto)
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
        public async Task<IActionResult> Edit(int id)
        {
            HttpResponseMessage response = await _httpClient.GetAsync($"/api/GetNewsDetail/{id}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var news = JsonConvert.DeserializeObject<NewsViewModel>(data);

                if (news != null)
                {
                    // Map NewsViewModel to your view model if necessary
                    var viewModel = new NewsViewModel
                    {
                        IdNew = news.IdNew,
                        Title = news.Title,
                        Content = news.Content,
                        PublishedDate = news.PublishedDate,
                        Cover = news.Cover
                    };

                    return View(viewModel);
                }
            }

            return RedirectToAction("ListNews");
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
                        return Json(new
                        {
                            id = news?.IdNew,
                            cover = news?.Cover, 
                            title = news?.Title,
                            content = news?.Content,
                            publishedDate = news?.PublishedDate
                        });
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

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var response = await _httpClient.DeleteAsync($"/api/DeleteNews/{id}");

                if (response.IsSuccessStatusCode)
                {

                    return Json(new { success = true });
                }


                return Json(new { success = false, message = "An error occurred while deleting the news." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An unexpected error occurred." });
            }
        }

    }
}