using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using API.Controllers;
using System.Net.Http.Json;
using API.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Test_SenShineSpa
{
    public class TestOfCard
    {
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            var appFactory = new WebApplicationFactory<CardController>();
            _client = appFactory.CreateClient();
        }

        [Test]
        public async Task CreateCard_ShouldReturnSuccess()
        {
            DateTime localDate = DateTime.Now;
            // Arrange
            var cardDTO = new
            {
                CardNumber = "testCard" + localDate.ToString(),
                BranchId = 1,
                CustomerId = 1,
                Status = "Active"
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Card/Create", cardDTO);

            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode, "Failed to create card");
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("Tạo thẻ"), "Card creation message mismatch");
        }

        [Test]
        public async Task GetAllCards_ShouldReturnCards()
        {
            // Act
            var response = await _client.GetAsync("/api/Card/GetAll");

            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode, "Failed to retrieve cards");
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(content, "No content retrieved");
        }

        [Test]
        public async Task GetCardById_ShouldReturnCard()
        {
            // Arrange
            var cardId = 1;

            // Act
            var response = await _client.GetAsync($"/api/Card/GetById?id={cardId}");

            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode, "Failed to retrieve card");
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(content, "No content retrieved");
        }

        [Test]
        public async Task UpdateCard_ShouldReturnSuccess()
        {
            // Arrange
            var cardId = 1;
            var cardDTO = new
            {
                CardNumber = "aaa",
                CustomerId = 2,
                Status = "Deactive",
            };

            var response = await _client.PutAsJsonAsync($"/api/Card/Update?id={cardId}", cardDTO);

            var content = await response.Content.ReadAsStringAsync();
            if (!response.IsSuccessStatusCode)
            {
                Assert.Fail($"Request failed with status code {response.StatusCode}: {content}");
            }
            Assert.IsTrue(content.Contains("thành công"), "Card update message mismatch");
        }

        [Test]
        public async Task ActiveDeactiveCard_ShouldReturnSuccess()
        {
            // Arrange
            var cardId = 1;

            // Act
            var response = await _client.PutAsync($"/api/Card/ActiveDeactive?id={cardId}", null);

            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode, "Failed to change card status");
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("Chuyển trạng thái thẻ"), "Card status change message mismatch");
        }

        [Test]
        public async Task GetCardComboByCard_ShouldReturnCombos()
        {
            // Arrange
            var cardId = 1;

            // Act
            var response = await _client.GetAsync($"/api/Card/GetCardComboByCard?id={cardId}");

            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode, "Failed to retrieve card combos");
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(content, "No content retrieved");
        }

        [Test]
        public async Task AddCardCombo_ShouldReturnSuccess()
        {
            // Arrange
            var cardComboDTO = new
            {
                CardId = 1,
                ComboId = 1,
                Quantity = 5
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/Card/AddCombo", cardComboDTO);

            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode, "Failed to add card combo");
            var content = await response.Content.ReadAsStringAsync();
            Assert.IsTrue(content.Contains("Thêm combo thành công"), "Card combo addition message mismatch");
        }
    }
}
