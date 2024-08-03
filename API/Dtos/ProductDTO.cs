using API.Models;

namespace API.Dtos
{
    public class ProductDTO
    {
        
        public string ProductName { get; set; } = null!;
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public virtual ICollection<CategoryDTO> Categories { get; set; } = new List<CategoryDTO>();
    }
    public class ProductDTORequest
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public virtual ICollection<CategoryDTO> Categories { get; set; } = new List<CategoryDTO>();
    }
    public class ProductDTORequest_2
    {
        public string ProductName { get; set; }
        public decimal? Price { get; set; }
        public int? Quantity { get; set; }
        public ICollection<int> CategoryIds { get; set; }
       // public ICollection<int> ImageIds { get; set; }
    }
}
