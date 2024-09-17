using API.Models;

namespace API.Dtos
{
    public class AppointmentDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int EmployeeId { get; set; }
        public int BedId { get; set; }
        public int SlotId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public string? Status { get; set; }
        public List<int>? ComboIDs { get; set; }
        public List<int>? ServiceIDs { get; set; }
    }
}