using System.ComponentModel;

namespace Web.Models
{
    public class SalaryViewModel
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        [DisplayName("Lương cơ bản")]
        public decimal? BaseSalary { get; set; }
        [DisplayName("Trợ cấp")]
        public decimal? Allowances { get; set; }
        [DisplayName("Thưởng")]
        public decimal? Bonus { get; set; }
        [DisplayName("Phạt")]
        public decimal? Deductions { get; set; }
        [DisplayName("Lương thực nhận")]
        public decimal? TotalSalary { get; set; }
        [DisplayName("Tháng")]
        public int? SalaryMonth { get; set; }
        [DisplayName("Năm")]
        public int? SalaryYear { get; set; }
        [DisplayName("Nhân viên")]
        public string? EmployeeName { get; set; }
        public string? BranchName { get; set; }
    }
}
