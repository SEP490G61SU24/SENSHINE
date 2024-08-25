using System;
using System.Collections.Generic;

namespace API.Models
{
    public partial class Spa
    {
        public Spa()
        {
            Cards = new HashSet<Card>();
            Invoices = new HashSet<Invoice>();
            Products = new HashSet<Product>();
            Promotions = new HashSet<Promotion>();
            Rooms = new HashSet<Room>();
        }

        public int Id { get; set; }
        public string SpaName { get; set; } = null!;
        public string? ProvinceCode { get; set; }
        public string? DistrictCode { get; set; }
        public string? WardCode { get; set; }

        public virtual ICollection<Card> Cards { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<Promotion> Promotions { get; set; }
        public virtual ICollection<Room> Rooms { get; set; }
    }
}
