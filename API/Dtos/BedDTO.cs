using System;

namespace API.Models
{
    public class BedDTO
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string BedNumber { get; set; } = null!;
        public string StatusWorking { get; set; } = null!;
        
    }
}
