using System;
using System.Collections.Generic;

namespace API.Models
{
    public partial class Invoice
    {
        public Invoice()
        {
            CardInvoices = new HashSet<CardInvoice>();
            InvoiceCombos = new HashSet<InvoiceCombo>();
            InvoiceServices = new HashSet<InvoiceService>();
        }

        public int Id { get; set; }
        public int? SpaId { get; set; }
        public int? CustomerId { get; set; }
        public int? PromotionId { get; set; }
        public decimal? Amount { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string Status { get; set; } = null!;
        public string? Description { get; set; }

        public virtual User? Customer { get; set; }
        public virtual Promotion? Promotion { get; set; }
        public virtual Spa? Spa { get; set; }
        public virtual ICollection<CardInvoice> CardInvoices { get; set; }
        public virtual ICollection<InvoiceCombo> InvoiceCombos { get; set; }
        public virtual ICollection<InvoiceService> InvoiceServices { get; set; }
    }
}
