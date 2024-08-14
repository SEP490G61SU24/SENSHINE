using API.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Test_SenShineSpa
{
    internal class TestOfService
    {
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            var appFactory = new WebApplicationFactory<API.Controllers.ServiceController>();
            _client = appFactory.CreateClient();
        }

        [Test]
        public async Task GetAllServices_ShouldReturnAllServices()
        {
            // Act
            var response = await _client.GetAsync("/api/Service/GetAllServices");

            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode, $"GET request failed with status code {response.StatusCode}");

            var content = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(content, "Response content is null");

            var services = JsonConvert.DeserializeObject<List<ServiceDTO>>(content);
            Assert.IsNotEmpty(services, "No services found");
        }

        [Test]
        public async Task GetByID_ShouldReturnServiceDetails()
        {
            // Arrange
            int serviceId = 1; // ID của dịch vụ tồn tại trong cơ sở dữ liệu của bạn

            // Act
            var response = await _client.GetAsync($"/api/Service/GetByID?Id={serviceId}");

            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode, $"GET request failed with status code {response.StatusCode}");

            var content = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(content, "Response content is null");

            var service = JsonConvert.DeserializeObject<ServiceDTO>(content);
            Assert.IsNotNull(service, "Service data is null");
            Assert.AreEqual(serviceId, service.Id, "Service ID does not match");
        }
        [Test]
        public async Task UpdateService_ShouldReturnUpdatedService()
        {
            // Arrange
            int serviceId = 1; // ID của dịch vụ cần cập nhật
            var updateServiceDto = new ServiceDTO
            {
                ServiceName = "Updated Service Name",
                Amount = 150,
                Description = "Updated Description"
            };

            var jsonContent = JsonConvert.SerializeObject(updateServiceDto);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act - Cập nhật dịch vụ
            var putResponse = await _client.PutAsync($"/api/Service/UpdateService?id={serviceId}", content);

            // Assert - Kiểm tra phản hồi từ PUT
            Assert.IsTrue(putResponse.IsSuccessStatusCode, $"PUT request failed with status code {putResponse.StatusCode}");

            // Act - Lấy thông tin dịch vụ đã cập nhật
            var getResponse = await _client.GetAsync($"/api/Service/GetByID?Id={serviceId}");

            // Assert - Kiểm tra phản hồi từ GET
            Assert.IsTrue(getResponse.IsSuccessStatusCode, $"GET request failed with status code {getResponse.StatusCode}");

            var getContent = await getResponse.Content.ReadAsStringAsync();
            var updatedService = JsonConvert.DeserializeObject<ServiceDTO>(getContent);

            Assert.IsNotNull(updatedService, "Updated service data is null");
            Assert.AreEqual(updateServiceDto.ServiceName, updatedService.ServiceName, "ServiceName did not update correctly");
            Assert.AreEqual(updateServiceDto.Amount, updatedService.Amount, "Amount did not update correctly");
            Assert.AreEqual(updateServiceDto.Description, updatedService.Description, "Description did not update correctly");
        }

        [Test]
        public async Task CreateService_ShouldReturnSuccessMessage()
        {
            // Arrange
            var newServiceDto = new ServiceDTO
            {
                ServiceName = "New Service",
                Amount = 100,
                Description = "Description for new service"
            };

            var jsonContent = JsonConvert.SerializeObject(newServiceDto);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            // Act
            var response = await _client.PostAsync("/api/Service/Create", content);

            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode, $"POST request failed with status code {response.StatusCode}");

            var responseContent = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(responseContent, "Response content is null");

            // Optionally, you can assert specific response content or structure here
            Assert.IsTrue(responseContent.Contains("Tạo mới dịch vụ New Service thành công"), "Success message not found in response");
        }

    }
}
