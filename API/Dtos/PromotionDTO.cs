using API.Models;

namespace API.Dtos
{
    public class PromotionDTO
    {
        public PromotionDTO()
        {
            Invoices = new HashSet<Invoice>();
        }

        public int Id { get; set; }
        public int? SpaId { get; set; }
        public string? PromotionName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public decimal? DiscountPercentage { get; set; }

        public virtual Spa? Spa { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
    }
    public class PromotionDTORequest
    {
        
        public int? SpaId { get; set; }
        public string? PromotionName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public decimal? DiscountPercentage { get; set; }
       
    }
    public class PromotionDTORespond
    {
        public int Id { get; set; }
        public int? SpaId { get; set; }
        public string? PromotionName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Description { get; set; }
        public decimal? DiscountPercentage { get; set; }
        public string? SpaName { get; set; }
    }
  
}
