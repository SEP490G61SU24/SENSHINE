using API.Models;

namespace Web.Models
{
    public class ProductViewModel
    {
        public ProductViewModel()
        {
            
            Categories = new HashSet<Category>();
        }
        public int Id { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal? Price { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
    }
}
