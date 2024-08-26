using API.Dtos;
using Newtonsoft.Json;

namespace Web.Models
{
    public class InvoiceViewModel
    {
        public int Id { get; set; }
        public int? SpaId { get; set; }
        public int? CustomerId { get; set; }
        public int? PromotionId { get; set; }
        public decimal? Amount { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }


        public string? CustomerName { get; set; }
        public string? PromotionName { get; set; }
        public string? SpaName { get; set; }
        public decimal? DiscountPercentage { get; set; }


        //public ICollection<int> CardIds { get; set; } = new List<int>();
        public ICollection<int>? ComboIds { get; set; } = new List<int>();
        public ICollection<int>? ServiceIds { get; set; } = new List<int>();
        public string ComboIdsString { get; set; }
        public string ServiceIdsString { get; set; }
        public Dictionary<int, int?> ServiceQuantities { get; set; } = new Dictionary<int, int?>();
        public Dictionary<int, int?> ComboQuantities { get; set; } = new Dictionary<int, int?>();

        //public ICollection<CardDTO2>? Cards { get; set; } = new List<CardDTO2>();
        public ICollection<InvoiceComboDTO>? InvoiceCombos { get; set; } = new List<InvoiceComboDTO>();
        public ICollection<InvoiceServiceDTO>? InvoiceServices { get; set; } = new List<InvoiceServiceDTO>();
    }
    public class PaymentResponse
    {
        [JsonProperty("data")]
        public List<PaymentRecord> Data { get; set; }

        [JsonProperty("error")]
        public bool Error { get; set; }
    }

    public class PaymentRecord
    {
        [JsonProperty("content")]
        public string Content { get; set; }

        [JsonProperty("price")]
        public decimal Price { get; set; }

        [JsonProperty("createAt")]
        public DateTime CreateAt { get; set; }
    }
    public class PaymentViewModel
    {
        public List<PaymentRecord> PaymentRecords { get; set; }
    }

}
