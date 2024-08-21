﻿using System.Text.Json.Serialization;

namespace API.Dtos
{
    public class ComboDTO
    {
     
        public int Id { get; set; }
   
        public string? Name { get; set; }
      
        public int Quantity { get; set; }
   
        public string? Note { get; set; }
     
        public decimal? Price { get; set; }
    
        public decimal? Discount { get; set; }
      
        public decimal? SalePrice { get; set; }
      
        public virtual ICollection<ServiceDTO> Services { get; set; }
    }
    public class ComboDTO2
    {
        
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }
        public decimal? SalePrice { get; set; }

        public virtual ICollection<ServiceDTO> Services { get; set; }

    }
}
