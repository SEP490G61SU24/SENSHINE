namespace Web.Models
{
    public class BranchViewModel
    {
        public int Id { get; set; }
        public string SpaName { get; set; } = null!;
        public string? ProvinceCode { get; set; }
        public string? DistrictCode { get; set; }
        public string? WardCode { get; set; }
    }
}
