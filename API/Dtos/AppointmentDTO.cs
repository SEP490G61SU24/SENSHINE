using API.Models;

namespace API.Dtos
{
    public class AppointmentDTO
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string Status { get; set; }

        public string CustomerName { get; set; }
        public string EmployeeName { get; set; }
        public string ServiceName { get; set; }

        public virtual User? Customer { get; set; }
        public virtual User? Employee { get; set; }
        public virtual Room? Room { get; set; }
        public virtual Service? Service { get; set; }
        public virtual Spa? Spa { get; set; }
    }
}
