using System;
using System.Collections.Generic;

namespace API.Models
{
    public partial class Appointment
    {
        public Appointment()
        {
            Combos = new HashSet<Combo>();
            Services = new HashSet<Service>();
        }

        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int EmployeeId { get; set; }
        public int BedId { get; set; }
        public int SlotId { get; set; }
        public int? InvoiceId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string? Status { get; set; }

        public virtual Bed Bed { get; set; } = null!;
        public virtual User Customer { get; set; } = null!;
        public virtual User Employee { get; set; } = null!;
        public virtual Slot Slot { get; set; } = null!;

        public virtual ICollection<Combo> Combos { get; set; }
        public virtual ICollection<Service> Services { get; set; }
    }
}
