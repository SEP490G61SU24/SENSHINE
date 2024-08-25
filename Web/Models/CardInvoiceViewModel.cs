namespace Web.Models
{
    public class CardInvoiceViewModel
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public int InvoiceId { get; set; }

        public decimal? Amount { get; set; }
        public DateTime InvoiceDate { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? CustomerName { get; set; }
    }
}
