using NUnit.Framework;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using API;

namespace SenShineSpa
{
    public class TestOfProduct
    {
        private HttpClient _client;

        [SetUp]
        public void Setup()
        {
            var appFactory = new WebApplicationFactory<API.Controllers.ProductController>();
            _client = appFactory.CreateClient();
        }

        [Test]
        public async Task GetFilteredProducts_ShouldReturnFilteredProducts()
        {
            // Arrange
            var categoryName = "thuoc";
            var quantityRange = "1-10";
            var priceRange = "100-500";

            // Act
            var response = await _client.GetAsync($"/api/GetFilterProducts?categoryName={categoryName}&quantityRange={quantityRange}&priceRange={priceRange}");

            // Assert
            Assert.IsTrue(response.IsSuccessStatusCode, "Status code is not successful");

            var content = await response.Content.ReadAsStringAsync();
            Assert.IsNotNull(content, "Content is null");
            
        }

        [Test]
        public async Task GetFilteredProducts_ShouldReturnFilteredProductsWithDifferentParameters()
        {
            var testCases = new[]
            {
                new FilterTestCase { CategoryName = "thuoc", QuantityRange = "1-10", PriceRange = "100-500" },
                new FilterTestCase { CategoryName = "vat pham", QuantityRange = null, PriceRange = "10-50" },
                new FilterTestCase { CategoryName = null, QuantityRange = "5-15", PriceRange = null }
            };

            foreach (var testCase in testCases)
            {
                var response = await _client.GetAsync($"/api/GetFilterProducts?categoryName={testCase.CategoryName ?? ""}&quantityRange={testCase.QuantityRange ?? ""}&priceRange={testCase.PriceRange ?? ""}");
                Assert.IsTrue(response.IsSuccessStatusCode, $"Status code is not successful for {testCase}");

                var content = await response.Content.ReadAsStringAsync();
                Assert.IsNotNull(content, $"Content is null for {testCase}");
                
            }
        }

        public class FilterTestCase
        {
            public string CategoryName { get; set; }
            public string QuantityRange { get; set; }
            public string PriceRange { get; set; }
        }
    }
}
