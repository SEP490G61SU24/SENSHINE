using API.Dtos;


namespace Web.Models
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public virtual ICollection<CategoryDTO> Categories { get; set; } = new List<CategoryDTO>();
        public virtual ICollection<ProductImageDTO>? ProductImages { get; set; } = new List<ProductImageDTO>();
        public ICollection<int> CategoryIds { get; set; }
        public string CategoryIdsString { get; set; }
        public ICollection<string>? ImageUrls { get; set; }
    }
}
