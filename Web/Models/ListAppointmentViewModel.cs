using static Web.Models.AppointmentViewModel;
using System.ComponentModel;

namespace Web.Models
{
    public class ListAppointmentViewModel
    {
        [DisplayName("Appointment ID")]
        public int Id { get; set; }

        [DisplayName("Customer ID")]
        public int? CustomerId { get; set; }

        [DisplayName("Employee ID")]
        public int? EmployeeId { get; set; }

        [DisplayName("Appointment Date")]
        public DateTime AppointmentDate { get; set; }

        [DisplayName("Slot")]
        public string AppointmentSlot { get; set; }

        [DisplayName("Status")]
        public string Status { get; set; }
        [DisplayName("Room")]
        public string? Room { get; set; }

        [DisplayName("Bed")]
        public string? Bed { get; set; }

        public AppointmentUserViewModel Customer { get; set; }
        public AppointmentUserViewModel Employee { get; set; }
        public List<ServiceViewModel> Services { get; set; }

        public class RoomAppointmentViewModel
        {
            public int RoomId { get; set; }
            public string RoomName { get; set; }
        }
    }
}
