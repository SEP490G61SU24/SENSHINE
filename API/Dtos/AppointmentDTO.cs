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
        public ICollection<Combo>? Combos { get; set; }
        public ICollection<Service>? Services { get; set; }
    }
}