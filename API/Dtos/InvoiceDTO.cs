using API.Models;
using System;
using System.Collections.Generic;

namespace API.Dtos
{
    public class InvoiceDTO
    {
        public int Id { get; set; }
        public int? SpaId { get; set; }
        public int? CustomerId { get; set; }
        public int? PromotionId { get; set; }
        public decimal? Amount { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string? Description { get; set; }

        
        public string? CustomerName { get; set; }
        public string? PromotionName { get; set; }
        public string? SpaName { get; set; }


        public ICollection<int> CardIds { get; set; } = new List<int>();
        public ICollection<int> ComboIds { get; set; } = new List<int>();
        public ICollection<int> ServiceIds { get; set; } = new List<int>();


        public ICollection<CardDTO2>? Cards { get; set; } = new List<CardDTO2>();
        public ICollection<ComboDTO2>? Combos { get; set; } = new List<ComboDTO2>();
        public ICollection<ServiceDTO2>? Services { get; set; } = new List<ServiceDTO2>();
    }
}
