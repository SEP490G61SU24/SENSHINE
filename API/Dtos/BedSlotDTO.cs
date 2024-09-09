namespace API.Dtos
{
    public class BedSlotDTO
    {
        public int Id { get; set; }
        public int BedId { get; set; }
        public int SlotId { get; set; }
        public DateTime SlotDate { get; set; }
        public string? Status { get; set; }
    }
}
