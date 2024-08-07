﻿using System;
using System.Collections.Generic;

namespace API.Models
{
    public partial class Product
    {
        public Product()
        {
            Appointments = new HashSet<Appointment>();
            Categories = new HashSet<Category>();
            ProductImages = new HashSet<ProductImage>();
        }

        public int Id { get; set; }
        public string ProductName { get; set; } = null!;
        public decimal? Price { get; set; }
        public int Quantity { get; set; }

        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<Category> Categories { get; set; }
        public virtual ICollection<ProductImage>? ProductImages { get; set; }
    }
}
