using System;
using System.Collections.Generic;

namespace API.Models
{
    public partial class Combo
    {
        public Combo()
        {
            CardCombos = new HashSet<CardCombo>();
            InvoiceCombos = new HashSet<InvoiceCombo>();
            Appointments = new HashSet<Appointment>();
            Services = new HashSet<Service>();
        }

        public int Id { get; set; }
        public string? Name { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }
        public decimal? SalePrice { get; set; }

        public virtual ICollection<CardCombo> CardCombos { get; set; }
        public virtual ICollection<InvoiceCombo> InvoiceCombos { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<Service> Services { get; set; }
    }
}
