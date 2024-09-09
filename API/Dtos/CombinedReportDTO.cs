namespace API.Dtos
{
    public class CombinedReportDTO
    {
        public List<RevenueReport> RevenueReports { get; set; }
        public List<DiscountRevenueReport> DiscountRevenueReports { get; set; }
        public List<RevenueReport> InvoiceStatusSummary { get; set; } = null;
        public List<ServiceSummary> ServiceSummaries { get; set; }
        public List<ComboSummary> ComboSummaries { get; set; }
    }
}
