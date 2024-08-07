using System.Text.Json.Serialization;

namespace Web.Models
{
    public class ComboViewModel
    {

        [JsonPropertyName("id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string? Name { get; set; }
        [JsonPropertyName("quantity")]
        public int Quantity { get; set; }
        [JsonPropertyName("note")]
        public string? Note { get; set; }
        [JsonPropertyName("price")]
        public decimal? Price { get; set; }
        [JsonPropertyName("discount")]
        public decimal? Discount { get; set; }
        [JsonPropertyName("salePrice")]
        public decimal? SalePrice { get; set; }

        public List<int> SelectedServiceIds { get; set; } = new List<int>();

        public string SalePriceString { get; set; }
        public List<ServiceViewModel> AvailableServices { get; set; } = new List<ServiceViewModel>();

    }
}
