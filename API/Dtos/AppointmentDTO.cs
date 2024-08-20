using API.Models;

namespace API.Dtos
{
    public class AppointmentDTO
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string AppointmentSlot { get; set; }
        public string Status { get; set; }
        public string? RoomName { get; set; }
        public string? BedNumber { get; set; } 

        public virtual AppointmentUserDTO? Customer { get; set; }
        public virtual AppointmentUserDTO? Employee { get; set; }
        public virtual ICollection<ServiceDTO> Services { get; set; }
        public virtual ICollection<AppointmentProductDTO> Products { get; set; }


        public class AppointmentProductDTO
        {
            public int ProductId { get; set; }
            public string ProductName { get; set; }
        }
    }
}