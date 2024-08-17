using System;
using System.Collections.Generic;

namespace API.Models
{
    public partial class Service
    {
        public Service()
        {
            InvoiceServices = new HashSet<InvoiceService>();
            Reviews = new HashSet<Review>();
            Appointments = new HashSet<Appointment>();
            Combos = new HashSet<Combo>();
        }

        public int Id { get; set; }
        public string ServiceName { get; set; } = null!;
        public decimal Amount { get; set; }
        public string? Description { get; set; }

        public virtual ICollection<InvoiceService> InvoiceServices { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<Combo> Combos { get; set; }
    }
}
