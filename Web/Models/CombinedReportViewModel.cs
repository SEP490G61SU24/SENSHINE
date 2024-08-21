using API.Dtos;

namespace Web.Models
{
    public class CombinedReportViewModel
    {
        public List<RevenueReport> RevenueReports { get; set; }
        public List<RevenueReport> InvoiceStatusSummary { get; set; }
        public List<ServiceSummary> ServiceSummaries { get; set; }
        public List<ComboSummary> ComboSummaries { get; set; }
    }
 
}