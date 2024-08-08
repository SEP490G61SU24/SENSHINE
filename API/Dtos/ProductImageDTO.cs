using API.Models;

namespace API.Dtos
{
    public class ProductImageDTO
    {
        public int Id { get; set; }
        public int? ProductId { get; set; }
        public string? ImageUrl { get; set; }
       
    }
}
