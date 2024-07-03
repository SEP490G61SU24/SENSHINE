using API.Models;

namespace API.Dtos
{
    public class ServiceDTO
    {
        public int Id { get; set; }
        public string ServiceName { get; set; } = null!;
        public decimal Amount { get; set; }
        public string? Description { get; set; }

    }
}
