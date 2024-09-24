namespace Web.Models
{
    public class UserSlotViewModel
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string? UserName { get; set; }
        public int SlotId { get; set; }
        public string? SlotName { get; set; }
        public DateTime SlotDate { get; set; }
        public string? Status { get; set; }
    }
}
