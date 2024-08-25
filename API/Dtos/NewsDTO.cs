
namespace API.Dtos
{
    public class NewsDTO
    {
        public int IdNew { get; set; }
        public string Title { get; set; } = null!;
        public string? Cover { get; set; } = null;
        public string Content { get; set; } = null!;
        public DateTime PublishedDate { get; set; }
        public IFormFile? CoverImage { get; set; }
    }
    public class NewsDTORequest
    {
        
        public string Title { get; set; } = null!;
        public string Cover { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string PublishedDate { get; set; }
    }
}
