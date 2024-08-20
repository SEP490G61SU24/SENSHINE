namespace API.Ultils
{
    public class FilteredPaginatedList<T> : PaginatedList<T>
    {
        public string? PriceRange { get; set; }
        public string? QuantityRange { get; set; }
        public string? CategoryName { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string? Status { get; set; }
    }
}
