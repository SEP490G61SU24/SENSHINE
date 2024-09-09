using System;
using System.Collections.Generic;

namespace API.Models
{
    public partial class InvoiceService
    {
        public int InvoiceId { get; set; }
        public int ServiceId { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; } 
        public virtual Invoice Invoice { get; set; } = null!;
        public virtual Service Service { get; set; } = null!;
    }
}
