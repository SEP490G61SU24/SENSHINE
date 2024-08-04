using System.ComponentModel;

namespace Web.Models
{
    public class BranchViewModel
    {
        public int Id { get; set; }
        [DisplayName("Tên Spa")]
        public string SpaName { get; set; } = null!;
        [DisplayName("Tỉnh")]
        public string? ProvinceCode { get; set; }
        [DisplayName("Huyện/Thành Phố")]
        public string? DistrictCode { get; set; }
        [DisplayName("Phường")]
        public string? WardCode { get; set; }
        public string? ProvinceName { get; set; }
        public string? DistrictName { get; set; }
        public string? WardName { get; set; }
    }
}
