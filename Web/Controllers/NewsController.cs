using API.Dtos;
using API.Models;
using API.Ultils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Text;
using Web.Models;
namespace Web.Controllers
{
    public class NewsController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<UserController> _logger;

        public NewsController(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<UserController> logger) : base(configuration, clientFactory, logger)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
        }
        


        [HttpGet]
        public async Task<IActionResult> NewsList(int pageIndex = 1, int pageSize = 10, string searchTerm = null)
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            var url = $"{apiUrl}/GetNewsPaging?pageIndex={pageIndex}&pageSize={pageSize}&searchTerm={searchTerm}";
            HttpResponseMessage response = await client.GetAsync(url);
            

            if (response.IsSuccessStatusCode)
            {
                var paginatedResult = await response.Content.ReadFromJsonAsync<PaginatedList<NewsViewModel>>();
                paginatedResult.SearchTerm = searchTerm;
                return View(paginatedResult);
            }
            else
            {
                return View("Error");
            }
        }


        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(NewsDTO newsDto)
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            if (!ModelState.IsValid)
            {
                return View(newsDto);
            }

            string json = JsonConvert.SerializeObject(newsDto);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync($"{apiUrl}/AddNews", content);

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
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/GetNewsDetail/{id}");

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
        public async Task<IActionResult> Edit(int id, NewsDTO newsDto)
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            if (!ModelState.IsValid)
            {
                var newsViewModel = new NewsViewModel
                {
                    IdNew = newsDto.IdNew,
                    Title = newsDto.Title,
                    Cover = newsDto.Cover,
                    Content = newsDto.Content,
                    PublishedDate = newsDto.PublishedDate
                };
                return View(newsViewModel);
            }

            string json = JsonConvert.SerializeObject(newsDto);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

            HttpResponseMessage response = await client.PutAsync($"{apiUrl}/EditNews/{id}", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("NewsList");
            }

            // Log error message here
            var newsViewModelError = new NewsViewModel
            {
                IdNew = newsDto.IdNew,
                Title = newsDto.Title,
                Cover = newsDto.Cover,
                Content = newsDto.Content,
                PublishedDate = newsDto.PublishedDate
            };
            ModelState.AddModelError(string.Empty, "An error occurred while editing the news.");
            return View(newsViewModelError);
        }


       

                [HttpGet]
                public async Task<IActionResult> GetNewsDetail(int id)
                {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/GetNewsDetail/{id}");

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
            var apiUrl = _configuration["ApiUrl"];
            string fromDateString = from.ToString("s");
            string toDateString = to.ToString("s");
            var client = _clientFactory.CreateClient();
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/NewsByDate?from={fromDateString}&to={toDateString}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var newsList = JsonConvert.DeserializeObject<IEnumerable<NewsViewModel>>(data);
                return Json(newsList);
            }

            return Json(new List<NewsViewModel>());
        }



        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            try
            {
                var response = await client.DeleteAsync($"{apiUrl}/DeleteNews/{id}");

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