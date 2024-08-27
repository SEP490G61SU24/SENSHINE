using API.Dtos;
using API.Models;
using API.Ultils;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
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
        public async Task<IActionResult> NewsList(int pageIndex = 1, int pageSize = 10, string searchTerm = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();
                var urlBuilder = new StringBuilder($"{apiUrl}/GetNewsPaging?");
                if (startDate != null)
                {
                    urlBuilder.Append($"startDate={startDate}&");
                }

                if (endDate != null)
                {
                    urlBuilder.Append($"endDate={endDate}&");
                }

                urlBuilder.Append($"pageIndex={pageIndex}&pageSize={pageSize}&searchTerm={searchTerm}");

                // Remove the trailing '&' if it exists
                var url = urlBuilder.ToString().TrimEnd('&');

                HttpResponseMessage response = await client.GetAsync(url);
            

                if (response.IsSuccessStatusCode)
                {
                    var paginatedResult = await response.Content.ReadFromJsonAsync<FilteredPaginatedList<NewsViewModel>>();
                    paginatedResult.SearchTerm = searchTerm;
                    return View(paginatedResult);
                }
                else
                {
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }

        protected async Task<UploadResponseDTO> UploadImageAsync(IFormFile image)
        {
            try
            {
                if (image == null || image.Length == 0)
                {
                    _logger.LogError("No image file provided for upload.");
                    return null;
                }
                var uploadEndpoint = $"https://apiupanh.kstest.pro/api/image/upload";

                using var client = _clientFactory.CreateClient();
                using var content = new MultipartFormDataContent();

                var fileContent = new StreamContent(image.OpenReadStream());
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("multipart/form-data");
                content.Add(fileContent, "image", image.FileName);

                var response = await client.PostAsync(uploadEndpoint, content);

                if (response.IsSuccessStatusCode)
                {
                    var responseData = await response.Content.ReadFromJsonAsync<UploadResponseDTO>();
                    return responseData;
                }
                else
                {
                    _logger.LogError($"Image upload failed. Status code: {response.StatusCode}");
                    return null;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError($"Image upload failed: {ex.Message}");
                return null;
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
            try
            {

                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();
                string imageUrl = null;

                if (newsDto.CoverImage != null && newsDto.CoverImage.Length > 0)
                {
                    var uploadResponse = await UploadImageAsync(newsDto.CoverImage);
                    if (uploadResponse != null && uploadResponse.Status)
                    {
                        imageUrl = uploadResponse.Data;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Image upload failed.");
                        return View(newsDto);
                    }
                }

                newsDto.Cover = imageUrl ?? newsDto.Cover;
                string json = JsonConvert.SerializeObject(newsDto);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync($"{apiUrl}/AddNews", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("NewsList");
                }

                _logger.LogError($"Failed to add news. Status code: {response.StatusCode}, Reason: {await response.Content.ReadAsStringAsync()}");
                ModelState.AddModelError(string.Empty, "An error occurred while adding the news.");
                return View(newsDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }



        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, NewsDTO newsDto)
        {
            try
            {
                // Check if the model state is valid
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

                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();
                string imageUrl = newsDto.Cover; // Default to existing Cover URL

                // Check if a new image is being uploaded
                if (newsDto.CoverImage != null && newsDto.CoverImage.Length > 0)
                {
                    var uploadResponse = await UploadImageAsync(newsDto.CoverImage);

                    if (uploadResponse != null && uploadResponse.Status)
                    {
                        imageUrl = uploadResponse.Data;
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Image upload failed.");
                        // Return the view with the model state errors
                        return View(newsDto);
                    }
                }

                // Update the Cover URL
                newsDto.Cover = imageUrl;
                newsDto.CoverImage = null;

                // Serialize the updated newsDto to JSON
                string json = JsonConvert.SerializeObject(newsDto);
                StringContent content = new StringContent(json, Encoding.UTF8, "application/json");

                // Make the PUT request to update the news
                HttpResponseMessage response = await client.PutAsync($"{apiUrl}/EditNews/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("NewsList");
                }
                else
                {
                    // Log the error response content
                    var errorMessage = await response.Content.ReadAsStringAsync();
                    _logger.LogError($"Error response from API: {errorMessage}");

                    // Return view with error message
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }


       

        [HttpGet]
        public async Task<IActionResult> GetNewsDetail(int id)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }

        [HttpGet]
        public async Task<IActionResult> NewsByDate(DateTime from, DateTime to)
        {
            try
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
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
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

        [HttpGet]
        public async Task<IActionResult> NewsDetail(int id)
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();
                HttpResponseMessage response = await client.GetAsync($"{apiUrl}/GetNewsDetail/{id}");

                if (response.IsSuccessStatusCode)
                {
                    var newsDetail = await response.Content.ReadFromJsonAsync<NewsViewModel>();
                    return View(newsDetail);
                }

                return View("Error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "CÓ LỖI XẢY RA!");
                ViewData["Error"] = "CÓ LỖI XẢY RA!";
                return View("Error");
            }
        }
    }
}