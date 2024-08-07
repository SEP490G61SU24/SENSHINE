﻿using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using System.Globalization;
using API.Dtos;

namespace Web.Controllers
{
	public class CustomerController : BaseController
	{
		private readonly IConfiguration _configuration;
		private readonly IHttpClientFactory _clientFactory;
		private readonly ILogger<CustomerController> _logger;

		public CustomerController(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<CustomerController> logger)
			 : base(configuration, clientFactory, logger)
		{
			_configuration = configuration;
			_clientFactory = clientFactory;
			_logger = logger;
		}

		public async Task<IActionResult> Index()
		{
			var apiUrl = _configuration["ApiUrl"];
			var client = _clientFactory.CreateClient();
			var response = await client.GetAsync($"{apiUrl}/user/byRole/5");
			if (response.IsSuccessStatusCode)
			{
				var users = await response.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>();
				return View(users);
			}
			else
			{
				return View("Error");
			}
		}

		public IActionResult Add()
		{
			return View();
		}

		[HttpPost]
		public async Task<IActionResult> Add(UserDTO user)
		{
			try
			{
				user.Password = "123456";

				string[] nameArr = user.FullName.Split(" ");

				if (nameArr.Length < 2)
				{
					user.FirstName = null;
					user.MidName = null;
					user.LastName = nameArr[0];
				}
				else if (nameArr.Length < 3)
				{
					user.FirstName = nameArr[0];
					user.MidName = null;
					user.LastName = nameArr[1];
				}
				else
				{
                    user.FirstName = nameArr[0];
                    user.LastName = nameArr[nameArr.Length - 1];
                    user.MidName = string.Join(" ", nameArr.Skip(1).Take(nameArr.Length - 2));
                }

				user.UserName = (RemoveDiacritics(user.LastName) + user.ProvinceCode + GenerateRandomString(4)).ToLower();
				user.RoleId = 5; // ROLE CUSTOMER

				var apiUrl = _configuration["ApiUrl"];

				var json = JsonSerializer.Serialize(user);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				using var client = _clientFactory.CreateClient();
				var response = await client.PostAsync($"{apiUrl}/user/add", content);

				if (response.IsSuccessStatusCode)
				{
					var responseString = await response.Content.ReadAsStringAsync();
					var responseData = JsonSerializer.Deserialize<UserDTO>(responseString);

					return RedirectToAction("Index", "Customer");
				}
				else
				{
					ViewData["Error"] = "Có lỗi xảy ra!";
					return View();
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error during login");
				ViewData["Error"] = "An error occurred";
				return View();
			}
		}

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var apiUrl = _configuration["ApiUrl"];
                var client = _clientFactory.CreateClient();
                var response = await client.GetAsync($"{apiUrl}/user/{id}");
                if (response.IsSuccessStatusCode)
                {
                    var user = await response.Content.ReadFromJsonAsync<UserDTO>();
                    user.FullName = string.Join(" ", new[] { user.FirstName, user.MidName, user.LastName }.Where(name => !string.IsNullOrEmpty(name)));
                    return View(user);
                }
                else
                {
                    return View("Error");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                ViewData["Error"] = "An error occurred";
                return View("Error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserDTO user)
        {
            try
            {
                string[] nameArr = user.FullName.Split(" ");

                if (nameArr.Length < 2)
                {
                    user.FirstName = null;
                    user.MidName = null;
                    user.LastName = nameArr[0];
                }
                else if (nameArr.Length < 3)
                {
                    user.FirstName = nameArr[0];
                    user.MidName = null;
                    user.LastName = nameArr[1];
                }
                else
                {
                    user.FirstName = nameArr[0];
                    user.LastName = nameArr[nameArr.Length - 1];
                    user.MidName = string.Join(" ", nameArr.Skip(1).Take(nameArr.Length - 2));
                }

                user.RoleId = 5; // CUSTOMER

                var apiUrl = _configuration["ApiUrl"];

                var json = JsonSerializer.Serialize(user);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                using var client = _clientFactory.CreateClient();
                var response = await client.PutAsync($"{apiUrl}/user/update/{user.Id}", content);

                if (response.IsSuccessStatusCode)
                {
                    var responseString = await response.Content.ReadAsStringAsync();
                    var responseData = JsonSerializer.Deserialize<UserDTO>(responseString);

                    //return View(responseData);
                    return RedirectToAction("Index", "Customer");
                }
                else
                {
                    ViewData["Error"] = "Có lỗi xảy ra!";
                    return View();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                ViewData["Error"] = "An error occurred";
                return View();
            }
        }

        public static string GenerateRandomString(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
			char[] stringChars = new char[length];

			for (int i = 0; i < length; i++)
			{
				stringChars[i] = chars[new Random().Next(chars.Length)];
			}

			return new string(stringChars);
		}

		public static string RemoveDiacritics(string text)
		{
			var normalizedString = text.Normalize(NormalizationForm.FormD);
			var stringBuilder = new StringBuilder();

			foreach (var c in normalizedString)
			{
				var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
				if (unicodeCategory != UnicodeCategory.NonSpacingMark)
				{
					stringBuilder.Append(c);
				}
			}

			return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
		}

	}
}
