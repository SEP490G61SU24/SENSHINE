using API.Dtos;
using API.Ultils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System;
using System.Text;
using Web.Models;

namespace Web.Controllers
{
    public class PromotionsController : BaseController
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<UserController> _logger;

        public PromotionsController(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<UserController> logger) : base(configuration, clientFactory, logger)
        {
            _configuration = configuration;
            _clientFactory = clientFactory;
            _logger = logger;
        }
        private async Task<UserViewModel> LoadUserAsync()
        {
            var user = new UserViewModel();
            var token = HttpContext.Session.GetString("Token");

            if (!string.IsNullOrEmpty(token))
            {
                var userProfile = await GetUserProfileAsync(token);

                if (userProfile != null)
                {
                    user.Id = userProfile.Id;
                    user.UserName = userProfile.UserName;
                    user.FirstName = userProfile.FirstName;
                    user.MidName = userProfile.MidName;
                    user.LastName = userProfile.LastName;
                    user.Phone = userProfile.Phone;
                    user.BirthDate = userProfile.BirthDate;
                    user.Status = userProfile.Status;
                    user.StatusWorking = userProfile.StatusWorking;
                    user.SpaId = userProfile.SpaId;
                    user.ProvinceCode = userProfile.ProvinceCode;
                    user.DistrictCode = userProfile.DistrictCode;
                    user.WardCode = userProfile.WardCode;
                    user.Address = userProfile.Address;
                    user.Roles = userProfile.Roles;
                    user.RoleName = userProfile.RoleName;
                    user.RoleId = userProfile.RoleId;
                    user.FullName = $"{userProfile.FirstName} {userProfile.MidName} {userProfile.LastName}";
                }
                else
                {
                    ViewData["Error"] = "Failed to retrieve user profile.";
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "An error occurred while fetching the user profile.");
            }

            return user;
        }
        [HttpGet]
        public async Task<IActionResult> ListPromotion(int? idspa, int pageIndex = 1, int pageSize = 10, string searchTerm = null, DateTime? startDate = null, DateTime? endDate = null)
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            var use = await LoadUserAsync();
            idspa = 1;
            var urlBuilder = new StringBuilder($"{apiUrl}/GetPromotionsPaging?");

            if (idspa != null)
            {
                urlBuilder.Append($"idspa={idspa}&");
            }

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
                var paginatedResult = await response.Content.ReadFromJsonAsync<PaginatedList<PromotionViewModel>>();
                paginatedResult.SearchTerm = searchTerm;
                return View(paginatedResult);
            }
            else
            {
                return View("Error");
            }
        }
        

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();

            // Create a SelectList to populate the dropdown
            var spaNames = await GetDistinctSpaNames();
            ViewBag.SpaList = new SelectList(spaNames, "Id", "SpaName");

            return View();

        }

        [HttpPost]
        public async Task<IActionResult> Add(PromotionDTORequest promotionDto)
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            if (!ModelState.IsValid)
            {
                return View(promotionDto);
            }

            string json = JsonConvert.SerializeObject(promotionDto);
            StringContent content = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync($"{apiUrl}/AddPromotion", content);

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("ListPromotion");
            }

            // Log error message here
            ModelState.AddModelError(string.Empty, "An error occurred while adding the news.");
            return View(promotionDto);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();

            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/GetPromotionDetail/{id}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var promotion = JsonConvert.DeserializeObject<PromotionViewModel>(data);

                if (promotion != null)
                {
                    // Map PromotionViewModel to your view model if necessary
                    var viewModel = new PromotionViewModel
                    {
                        Id = promotion.Id,
                        PromotionName = promotion.PromotionName,
                        Description = promotion.Description,
                        StartDate = promotion.StartDate,
                        EndDate = promotion.EndDate,
                        DiscountPercentage = promotion.DiscountPercentage,
                        SpaName = promotion.SpaName
                    };
                    var spaNames = await GetDistinctSpaNames();
                    ViewBag.SpaList = new SelectList(spaNames, "Id", "SpaName", promotion.SpaId);
                    return View(viewModel);
                }
            }

            return RedirectToAction("ListPromotion");
        }
        private async Task<List<BranchViewModel>> GetDistinctSpaNames()
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/Branch/GetAll");
            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var branches = JsonConvert.DeserializeObject<List<BranchViewModel>>(data);

                // Get distinct Spa names
                var distinctSpaNames = branches
                    .GroupBy(branch => branch.SpaName)
                    .Select(group => group.First())
                    .ToList();

                return distinctSpaNames;
            }

            return new List<BranchViewModel>();
        }

        [HttpPost]
        public async Task<IActionResult> Edit(PromotionViewModel promotionViewModel)
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            if (!ModelState.IsValid)
            {
                return View(promotionViewModel); 
            }

            try
            {
                var promotionDto = new PromotionDTO
                {
                    Id = promotionViewModel.Id,
                    SpaId = promotionViewModel.SpaId,
                    PromotionName = promotionViewModel.PromotionName,
                    StartDate = promotionViewModel.StartDate,
                    EndDate = promotionViewModel.EndDate,
                    Description = promotionViewModel.Description,
                    DiscountPercentage = promotionViewModel.DiscountPercentage
                };

                var content = new StringContent(JsonConvert.SerializeObject(promotionDto), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PutAsync($"{apiUrl}/EditPromotion/{promotionDto.Id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("ListPromotion"); // Redirect to list view on success.
                }

                var errorMessage = await response.Content.ReadAsStringAsync();
                ModelState.AddModelError(string.Empty, $"An error occurred: {errorMessage}"); // Add server error message.
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"Exception occurred: {ex.Message}"); // Handle unexpected exceptions.
            }

            return View(promotionViewModel); // Return view with validation errors.
        }


        [HttpGet]
        public async Task<IActionResult> GetPromotionDetail(int id)
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/GetPromotionDetail/{id}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var promotion = JsonConvert.DeserializeObject<PromotionViewModel>(data);

                return Json(new
                {
                    id = promotion?.Id,
                    promotionName = promotion?.PromotionName,
                    description = promotion?.Description,
                    startDate = promotion?.StartDate,
                    endDate = promotion?.EndDate,
                    discountPercentage = promotion?.DiscountPercentage,
                    spaName = promotion?.SpaName
                });
            }

            return NotFound();
        }


        [HttpGet]
        public async Task<IActionResult> Detail(int id)
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            HttpResponseMessage response = await client.GetAsync($"{apiUrl}/GetPromotionDetail/{id}");

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var promotionDto = JsonConvert.DeserializeObject<PromotionDTO>(data);
                return View(promotionDto);
            }

            return RedirectToAction("ListPromotion");
        }
        [HttpGet]
        public async Task<IActionResult> ListByFilter(string spaLocation, DateTime? startDate, DateTime? endDate)
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            // Construct the query string based on the parameters provided
            var query = new List<string>();

            if (!string.IsNullOrEmpty(spaLocation))
            {
                query.Add($"spaLocation={Uri.EscapeDataString(spaLocation)}");
            }

            if (startDate.HasValue)
            {
                query.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            }

            if (endDate.HasValue)
            {
                query.Add($"endDate={endDate.Value:yyyy-MM-dd}");
            }

            var queryString = string.Join("&", query);
            var url = $"{apiUrl}/GetPromotionsByFilter?{queryString}";

            HttpResponseMessage response = await client.GetAsync(url);

            if (response.IsSuccessStatusCode)
            {
                string data = await response.Content.ReadAsStringAsync();
                var promotions = JsonConvert.DeserializeObject<IEnumerable<PromotionDTORespond>>(data);
                return Json(promotions);
            }

            return StatusCode((int)response.StatusCode, "An error occurred while fetching promotions by filter.");
        }



        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var apiUrl = _configuration["ApiUrl"];
            var client = _clientFactory.CreateClient();
            try
            {
                var response = await client.DeleteAsync($"{apiUrl}/DeletePromotion/{id}");

                if (response.IsSuccessStatusCode)
                {
                
                    return Json(new { success = true });
                }

                
                return Json(new { success = false, message = "An error occurred while deleting the promotion." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An unexpected error occurred." });
            }
        }

    }
}

