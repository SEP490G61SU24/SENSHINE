using API.Models;

namespace API.Dtos
{
    public class CardComboDTO
    {
        public int Id { get; set; }
        public int CardId { get; set; }
        public int ComboId { get; set; }
        public int? SessionDone { get; set; }
    }
}
