namespace Web.Models
{
    public class CardViewModel
    {
        public int Id { get; set; }
        public string CardNumber { get; set; } = null!;
        public int CustomerId { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? Status { get; set; }
        public decimal? TotalPrice { get; set; }

        public List<int>? ComboId { get; set; }

        public List<int>? InvoiceId { get; set; }
    }
}