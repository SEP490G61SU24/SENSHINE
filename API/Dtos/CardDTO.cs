using API.Models;

namespace API.Dtos
{
    public class CardDTO
    {
        public int Id { get; set; }
        public string CardNumber { get; set; } = null!;
        public int CustomerId { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? Status { get; set; }
        public int? BranchId { get; set; }

        public List<int>? CardComboId { get; set; }
        public List<int>? InvoiceId { get; set; }
    }
    public class CardDTO2
    {
        public int Id { get; set; }
        public string CardNumber { get; set; } = null!;
        public int CustomerId { get; set; }
        public DateTime? CreateDate { get; set; }
        public string? Status { get; set; }
        public int? BranchId { get; set; }

        
    }
}
