namespace Web.Models
{
    public class ComboViewModelIndex
    {
        public int Id { get; set; }
        public string? Name { get; set; }
        public int Quantity { get; set; }
        public string? Note { get; set; }
        public decimal? Price { get; set; }
        public decimal? Discount { get; set; }
        public decimal? SalePrice { get; set; }
        public List<int> SelectedServiceIds { get; set; } = new List<int>();
        public List<ServiceViewModel> AvailableServices { get; set; } = new List<ServiceViewModel>();

        // New property to store service names
        public string ServiceNames => string.Join(", ", AvailableServices.Select(s => s.ServiceName));
    }
}
