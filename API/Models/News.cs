using System;
using System.Collections.Generic;

namespace API.Models
{
    public partial class News
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Cover { get; set; } = null!;
        public string Content { get; set; } = null!;
        public DateTime PublishedDate { get; set; }
    }
}
