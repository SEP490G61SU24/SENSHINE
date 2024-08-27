using System;
using System.Collections.Generic;

namespace API.Models
{
    public partial class Appointment
    {
        public Appointment()
        {
            Services = new HashSet<Service>();
        }

        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string AppointmentSlot { get; set; } = null!;
        public string? BedNumber { get; set; }
        public string? RoomName { get; set; }
        public string Status { get; set; } = null!;

        public virtual User Customer { get; set; } = null!;
        public virtual User Employee { get; set; } = null!;

        public virtual ICollection<Service> Services { get; set; }
    }
}
