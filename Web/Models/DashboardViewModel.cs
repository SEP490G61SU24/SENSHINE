namespace Web.Models
{
    public class DashboardViewModel
    {
        public List<string> Labels { get; set; } = new List<string>();
        public List<decimal> Values { get; set; } = new List<decimal>();

        // New properties for the salary chart
        public List<string> SalaryMonths { get; set; } = new List<string>();
        public List<decimal> TotalSalaries { get; set; } = new List<decimal>();
    }
}
