using API.Models;

namespace API.Dtos
{
    public class UserDto
    {
        public int Id { get; set; }
        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? FirstName { get; set; }
        public string? MidName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public DateTime? BirthDate { get; set; }
        public string? Status { get; set; }
        public string? StatusWorking { get; set; }
        public int? SpaId { get; set; }
        public string? ProvinceCode { get; set; }
        public string? DistrictCode { get; set; }
        public string? WardCode { get; set; }
        public string? Address { get; set; }
        public ICollection<RoleDTO>? Roles { get; set; }
        public string? RoleName { get; set; }
        public int RoleId { get; set; }
    }
}
