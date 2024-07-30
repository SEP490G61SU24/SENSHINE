using System.Text.Json.Serialization;

namespace Web.Models
{
    public class UserViewModel
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("userName")]
        public string? UserName { get; set; }

        [JsonPropertyName("password")]
        public string? Password { get; set; }

        [JsonPropertyName("firstName")]
        public string? FirstName { get; set; }

        [JsonPropertyName("midName")]
        public string? MidName { get; set; }

        [JsonPropertyName("lastName")]
        public string? LastName { get; set; }

        [JsonPropertyName("phone")]
        public string? Phone { get; set; }

        [JsonPropertyName("birthDate")]
        public DateTime? BirthDate { get; set; }

        [JsonPropertyName("provinceCode")]
        public string? ProvinceCode { get; set; }

        [JsonPropertyName("districtCode")]
        public string? DistrictCode { get; set; }

        [JsonPropertyName("wardCode")]
        public string? WardCode { get; set; }

        [JsonPropertyName("roleId")]
        public int RoleId { get; set; }
        public string? Address { get; set; }
        public string? FullName { get; set; }
        public string? RoleName { get; set; }
    }
}
