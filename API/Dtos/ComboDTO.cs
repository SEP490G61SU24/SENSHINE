﻿using System.Text.Json.Serialization;

namespace API.Dtos
{
    public class ComboDTO
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
        [JsonPropertyName("services")]
        public virtual ICollection<ServiceDTO> Services { get; set; }
    }
}
