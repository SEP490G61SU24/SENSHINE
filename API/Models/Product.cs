using System;
using System.Collections.Generic;

namespace API.Models
{
    public partial class Product
    {
        public Product()
        {
            ProductImages = new HashSet<ProductImage>();
            Appointments = new HashSet<Appointment>();
            Categories = new HashSet<Category>();
        }

        public int Id { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal? Price { get; set; }
        public int Quantity { get; set; }
        public int SpaId { get; set; }

        public virtual Spa Spa { get; set; } = null!;
        public virtual ICollection<ProductImage> ProductImages { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
    }
}
