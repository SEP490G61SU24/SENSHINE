using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using API;
using API.Dtos;
using System.Net.Http.Json;
using Newtonsoft.Json;
using System.Text;

namespace Test_SenShineSpa
{
    public class TestOfAppointment
    {
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            var appFactory = new WebApplicationFactory<API.Controllers.AppointmentController>();
            _client = appFactory.CreateClient();
        }

        [Test]
        public async Task UpdateAppointment_ShouldReturnAllAppointments()
        {
            // Act
            var response = await _client.GetAsync("/api/Appointment/GetAllAppointments");

            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode, "Status code is not successful");

            var content = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(content, "Content is null");

        }


        [Test]
        public async Task CreateAppointment_ShouldCreateNewAppointment()
        {
            // Arrange
            var newAppointment = new AppointmentDTO
            {
                CustomerId = 1, // Sử dụng ID hợp lệ cho khách hàng
                EmployeeId = 1, // Sử dụng ID hợp lệ cho nhân viên
                AppointmentDate = DateTime.Now,
                Status = "Pending",
                Services = new List<ServiceDTO>
                {
                    new ServiceDTO { Id = 1, ServiceName = "Massage", Amount = 100, Description = "Relaxing massage" }
                },
                Products = new List<AppointmentDTO.AppointmentProductDTO>
                {
                    new AppointmentDTO.AppointmentProductDTO { ProductId = 1, ProductName = "Essential Oil" }
                }
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Appointment/Create", newAppointment);

            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode, "Status code is not successful");

            var content = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(content, "Content is null");
        }
    }
}
