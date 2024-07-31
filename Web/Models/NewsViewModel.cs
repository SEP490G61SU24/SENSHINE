namespace Web.Models
{
    public class NewsViewModel
    {
        public int IdNew { get; set; }
        public string Title { get; set; } = null!;
        public string Cover { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime PublishedDate { get; set; }
        
    }
}
