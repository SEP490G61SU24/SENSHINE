﻿using System;
using System.Collections.Generic;

namespace API.Models
{
    public partial class Appointment
    {
        public Appointment()
        {
            Products = new HashSet<Product>();
            Services = new HashSet<Service>();
        }

        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public bool Status { get; set; }

        public virtual User Customer { get; set; } = null!;
        public virtual User Employee { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Service> Services { get; set; }
    }
}
