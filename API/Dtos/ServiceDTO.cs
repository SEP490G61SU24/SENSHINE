﻿using API.Models;

namespace API.Dtos
{
    public class ServiceDTO
    {
        public int IdSer { get; set; }
        public string ServiceName { get; set; } = null!;
        public decimal Price { get; set; }
        public string? Description { get; set; }

    }
}
