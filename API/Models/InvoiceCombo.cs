using System;
using System.Collections.Generic;

namespace API.Models
{
    public partial class InvoiceCombo
    {
        public int InvoiceId { get; set; }
        public int ComboId { get; set; }
        public int? Quantity { get; set; }
        public decimal? Price { get; set; } 

        public virtual Combo Combo { get; set; } = null!;
        public virtual Invoice Invoice { get; set; } = null!;
    }
}
