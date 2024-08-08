namespace Web.Models
{
    public class BedViewModel
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string BedNumber { get; set; } = null!;
        public string StatusWorking { get; set; } = null!;
    }
}
