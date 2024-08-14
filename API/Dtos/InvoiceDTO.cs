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
		public string? Status { get; set; }


		public string? CustomerName { get; set; }
        public string? PromotionName { get; set; }
        public string? SpaName { get; set; }
        public decimal? DiscountPercentage { get; set; }


        /*public ICollection<int> CardIds { get; set; } = new List<int>();*/
        public ICollection<int> ComboIds { get; set; } = new List<int>();
        public ICollection<int> ServiceIds { get; set; } = new List<int>();
        public Dictionary<int, int?>? ServiceQuantities { get; set; }
        public Dictionary<int, int?>? ComboQuantities { get; set; }

        //public ICollection<CardDTO2>? Cards { get; set; } = new List<CardDTO2>();
        public ICollection<InvoiceComboDTO>? InvoiceCombos { get; set; }
        public ICollection<InvoiceServiceDTO>? InvoiceServices { get; set; }
        public ICollection<CardDTO2>? Cards { get; set; }
    }
        public class InvoiceDTO2
        {
            public int Id { get; set; }
            public int? IdCombo { get; set; }
            public string? NameCombo { get; set; }
            public int? IdService { get; set; }
            public string? NameService { get; set; }
       
        }
    
        public class InvoiceComboDTO
        {
            public int InvoiceId { get; set; }
            public int ComboId { get; set; }
            public int? Quantity { get; set; }

            
            public ComboDTO2? Combo { get; set; }
        }


        public class InvoiceServiceDTO
        {
            public int InvoiceId { get; set; }
            public int ServiceId { get; set; }
            public int? Quantity { get; set; }

            
            public ServiceDTO2? Service { get; set; }
        }
    }


