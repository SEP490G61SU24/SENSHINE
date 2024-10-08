﻿using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace Web.Models
{
    public class AppointmentViewModel
    {

        [DisplayName("Appointment ID")]
        public int Id { get; set; }

        [DisplayName("Customer ID")]
        public int? CustomerId { get; set; }

        [DisplayName("Employee ID")]
        public int? EmployeeId { get; set; }

        [DisplayName("Appointment Date")]
        public DateTime? AppointmentDate { get; set; }
        [DisplayName("Slot")]
        public string AppointmentSlot { get; set; }
        public int BedId { get; set; }
        public string? RoomName { get; set; }
        [DisplayName("Status")]
        public string Status { get; set; }
        public List<int> SelectedServiceIds { get; set; } = new List<int>();
        public List<int> SelectedProductIds { get; set; } = new List<int>();

        public class AppointmentUserViewModel
        {
            [DisplayName("User ID")]
            public int Id { get; set; }

            [DisplayName("Full Name")]
            public string FullName { get; set; }

            [DisplayName("Phone")]
            public string Phone { get; set; }

            [DisplayName("Address")]
            public string Address { get; set; }
            public int? SpaId { get; set; }
        }

    }

}
