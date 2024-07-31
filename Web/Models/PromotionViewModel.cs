        namespace Web.Models
        {
            public class PromotionViewModel
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
