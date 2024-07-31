using API.Models;

namespace API.Dtos
{
    public class AppointmentUserDTO
    {
        public int Id { get; set; }
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }

    }
}
