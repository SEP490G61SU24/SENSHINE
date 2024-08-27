namespace API.Dtos
{
    public class AppointmentDTO
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int EmployeeId { get; set; }
        public DateTime? AppointmentDate { get; set; }
        public string AppointmentSlot { get; set; }
        public string? BedNumber { get; set; } 
        public string? RoomName { get; set; }
        public string Status { get; set; }

        public virtual UserDTO? Customer { get; set; }
        public virtual UserDTO? Employee { get; set; }
        public virtual ICollection<ServiceDTO> Services { get; set; }
    }
}