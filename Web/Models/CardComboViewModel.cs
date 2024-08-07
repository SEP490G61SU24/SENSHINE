namespace Web.Models
{
    public class CardComboViewModel
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public int ComboId { get; set; }
        public int? SessionDone { get; set; }
        public int? SessionLeft { get; set; }
        public string ComboName { get; set; }
    }
}
