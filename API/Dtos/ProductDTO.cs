    using API.Models;
using System.ComponentModel.DataAnnotations;

    namespace API.Dtos
    {
        public class ProductDTO
        {
        
            public string ProductName { get; set; } = null!;
            public decimal? Price { get; set; }
            public int? Quantity { get; set; }
            public List<int> CategoryIds { get; set; } = new List<int>();
            public virtual ICollection<ProductImageDTO>? ImageUrls { get; set; } = new List<ProductImageDTO>();
        }
        public class ProductDTORequest
        {
            public int Id { get; set; }
            public string ProductName { get; set; } = null!;
            public decimal? Price { get; set; }
            public int? Quantity { get; set; }
            public virtual ICollection<CategoryDTO> Categories { get; set; } = new List<CategoryDTO>();
            public virtual ICollection<ProductImageDTO>? ProductImages { get; set; } = new List<ProductImageDTO>();
            
    }
        public class ProductDTORequest_2
        {
            public string ProductName { get; set; } = null!;
            public decimal? Price { get; set; }
            public int? Quantity { get; set; }
            public ICollection<int> CategoryIds { get; set; } = null!;
            public ICollection<string>? ImageUrls { get; set; }

        }

    }
