﻿namespace API.Dtos
{
    public class RoleDTO
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = null!;
        public string Rules { get; set; } = null!;
    }
}
