namespace Web.Models
{
    public class BedViewModelIndex
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string BedNumber { get; set; } = null!;
        public string StatusWorking { get; set; } = null!;
        public string RoomName { get; set; } // Thêm thuộc tính này
    }
}

