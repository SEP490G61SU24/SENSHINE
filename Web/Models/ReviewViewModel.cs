namespace Web.Models
{
    public class ReviewViewModel
    {
        public int Id { get; set; }
        public int? CustomerId { get; set; }
        public int? ServiceId { get; set; }
        public int? Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime? ReviewDate { get; set; }
    }
}
