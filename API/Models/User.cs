using System;
using System.Collections.Generic;

namespace API.Models
{
    public partial class User
    {
        public User()
        {
            AppointmentCustomers = new HashSet<Appointment>();
            AppointmentEmployees = new HashSet<Appointment>();
            Cards = new HashSet<Card>();
            Invoices = new HashSet<Invoice>();
            Notifications = new HashSet<Notification>();
            Reviews = new HashSet<Review>();
            Salaries = new HashSet<Salary>();
            UserSlots = new HashSet<UserSlot>();
            WorkSchedules = new HashSet<WorkSchedule>();
            Roles = new HashSet<Role>();
        }

        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? FirstName { get; set; }
        public string? MidName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public DateTime? BirthDate { get; set; }
        public string Status { get; set; } = null!;
        public string StatusWorking { get; set; } = null!;
        public int? SpaId { get; set; }
        public string? ProvinceCode { get; set; }
        public string? DistrictCode { get; set; }
        public string? WardCode { get; set; }
        public DateTime? CreatedAt { get; set; }

        public virtual ICollection<Appointment> AppointmentCustomers { get; set; }
        public virtual ICollection<Appointment> AppointmentEmployees { get; set; }
        public virtual ICollection<Card> Cards { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
        public virtual ICollection<Salary> Salaries { get; set; }
        public virtual ICollection<UserSlot> UserSlots { get; set; }
        public virtual ICollection<WorkSchedule> WorkSchedules { get; set; }

        public virtual ICollection<Role> Roles { get; set; }
    }
}
