using System;
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
        public DateTime AppointmentDate { get; set; }

        [DisplayName("Status")]
        public bool Status { get; set; }

        public AppointmentUserViewModel Customer { get; set; }
        public AppointmentUserViewModel Employee { get; set; }
        public List<ServiceViewModel> Services { get; set; }
        public List<ProductViewModel> Products { get; set; }
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
        }
    }

}
