using API.Models;

namespace API.Dtos
{
    public class SalaryDTO
    {
        public int Id { get; set; }
        public int EmployeeId { get; set; }
        public decimal? BaseSalary { get; set; }
        public decimal? Allowances { get; set; }
        public decimal? Bonus { get; set; }
        public decimal? Deductions { get; set; }
        public decimal? TotalSalary { get; set; }
        public int? SalaryMonth { get; set; }
        public int? SalaryYear { get; set; }
    }
}
