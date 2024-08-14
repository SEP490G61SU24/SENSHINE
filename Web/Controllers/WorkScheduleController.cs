﻿using API.Dtos;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using System.Text.Json;
using Web.Models;

namespace Web.Controllers
{
	public class WorkScheduleController : BaseController
	{
		private readonly IConfiguration _configuration;
		private readonly IHttpClientFactory _clientFactory;
		private readonly ILogger<WorkScheduleController> _logger;

		public WorkScheduleController(IConfiguration configuration, IHttpClientFactory clientFactory, ILogger<WorkScheduleController> logger)
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
			var response = await client.GetAsync($"{apiUrl}/work-schedules");
			if (response.IsSuccessStatusCode)
			{
				var data = await response.Content.ReadFromJsonAsync<IEnumerable<WorkScheduleDTO>>();
				return View(data);
			}
			else
			{
				return View("Error");
			}
		}

		public async Task<IActionResult> Add()
		{
			try
			{
				var apiUrl = _configuration["ApiUrl"];
				var client = _clientFactory.CreateClient();

				// Lấy tất cả emp
				var employeeResponse = await client.GetAsync($"{apiUrl}/user/byRole/4");
				if (!employeeResponse.IsSuccessStatusCode)
				{
					return View("Error");
				}
				var employeeData = await employeeResponse.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>();

				var viewData = new WorkScheduleViewModel
				{
					Employees = employeeData,
				};

				return View(viewData);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error during login");
				ViewData["Error"] = "An error occurred";
				return View("Error");
			}
		}

		[HttpPost]
		public async Task<IActionResult> Add(WorkScheduleViewModel model)
		{
			try
			{
				var apiUrl = _configuration["ApiUrl"];
				using var client = _clientFactory.CreateClient();
				var employeeResponse = await client.GetAsync($"{apiUrl}/user/byRole/4");
				var employeeData = await employeeResponse.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>();

				if (!ModelState.IsValid)
				{
					model.Employees = employeeData;

					ViewData["Error"] = "Dữ liệu đầu vào thiếu!";

					return View(model);
				}

				if (model.StartDateTime >= model.EndDateTime)
				{
					model.Employees = employeeData;
					ViewData["Error"] = "Giờ kết thúc phải sau giờ bắt đầu.";
					return View(model);
				}

				WorkScheduleDTO data = new WorkScheduleDTO
				{
					EmployeeId = model.EmployeeId,
					StartDateTime = model.StartDateTime,
					EndDateTime = model.EndDateTime,
					Status = model.Status
				};

				var json = JsonSerializer.Serialize(data);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				var response = await client.PostAsync($"{apiUrl}/work-schedules", content);

				if (response.IsSuccessStatusCode)
				{
					ViewData["SuccessMsg"] = "Thêm thành công!";
					return RedirectToAction("Index", "workschedule");
				}
				else
				{
					model.Employees = employeeData;

					ViewData["Error"] = "Có lỗi xảy ra!";
					return View(model);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error during work schedule creation");
				ViewData["Error"] = "An error occurred";
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

				// Lấy tất cả emp
				var employeeResponse = await client.GetAsync($"{apiUrl}/user/byRole/4");
				if (!employeeResponse.IsSuccessStatusCode)
				{
					return View("Error");
				}
				var employeeData = await employeeResponse.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>();

				// Lấy ws
				var wsResponse = await client.GetAsync($"{apiUrl}/work-schedules/{id}");
				if (!wsResponse.IsSuccessStatusCode)
				{
					return View("Error");
				}
				var wsData = await wsResponse.Content.ReadFromJsonAsync<WorkScheduleDTO>();

				var viewData = new WorkScheduleViewModel
				{
					Id = id,
					Employees = employeeData,
					WorkScheduleData = wsData,
				};

				return View(viewData);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error during login");
				ViewData["Error"] = "An error occurred";
				return View("Error");
			}
		}

		[HttpPost]
		public async Task<IActionResult> Edit(WorkScheduleViewModel model)
		{
			try
			{
				var apiUrl = _configuration["ApiUrl"];
				using var client = _clientFactory.CreateClient();
				var employeeResponse = await client.GetAsync($"{apiUrl}/user/byRole/4");
				var employeeData = await employeeResponse.Content.ReadFromJsonAsync<IEnumerable<UserDTO>>();

				// Lấy ws
				var wsResponse = await client.GetAsync($"{apiUrl}/work-schedules/{model.Id}");
				if (!wsResponse.IsSuccessStatusCode)
				{
					return View("Error");
				}
				var wsData = await wsResponse.Content.ReadFromJsonAsync<WorkScheduleDTO>();

				if (!ModelState.IsValid)
				{
					model.WorkScheduleData = wsData;
					model.Employees = employeeData;

					ViewData["Error"] = "Dữ liệu đầu vào thiếu!";

					return View(model);
				}

				WorkScheduleDTO data = new WorkScheduleDTO
				{
					Id = model.Id,
					EmployeeId = model.EmployeeId,
					StartDateTime = model.StartDateTime,
					EndDateTime = model.EndDateTime,
					Status = model.Status
				};

				var json = JsonSerializer.Serialize(data);
				var content = new StringContent(json, Encoding.UTF8, "application/json");

				var response = await client.PutAsync($"{apiUrl}/work-schedules/{model.Id}", content);

				if (response.IsSuccessStatusCode)
				{
					ViewData["Success"] = "Cập nhật thành công!";
					return RedirectToAction("Index", "WorkSchedule");
				}
				else
				{
					model.WorkScheduleData = wsData;
					model.Employees = employeeData;

					ViewData["Error"] = "Có lỗi xảy ra!";
					return View(model);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error during work schedule creation");
				ViewData["Error"] = "An error occurred";
				return View("Error");
			}
		}

		[HttpDelete]
		public async Task<IActionResult> Delete(string id)
		{
			try
			{
				var apiUrl = _configuration["ApiUrl"];
				using var client = _clientFactory.CreateClient();
				var response = await client.DeleteAsync($"{apiUrl}/work-schedules/{id}");

				if (response.IsSuccessStatusCode)
				{
					ViewData["SuccessMsg"] = "Xóa thành công!";
					return RedirectToAction("Index", "workschedule");
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
				ViewData["Error"] = "Có lỗi xảy ra!";
				return View();
			}
		}
	}
}