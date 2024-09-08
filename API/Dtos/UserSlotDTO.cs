namespace API.Dtos
{
    public class UserSlotDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int SlotId { get; set; }
        public DateTime SlotDate { get; set; }
        public string? Status { get; set; }
    }
}
