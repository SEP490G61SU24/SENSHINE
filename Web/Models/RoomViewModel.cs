using System.ComponentModel;

namespace Web.Models
{
    public class RoomViewModel
    {
        public int Id { get; set; }
        public int? SpaId { get; set; }
        [DisplayName("Tên phòng")]
        public string RoomName { get; set; } = null!;
        [DisplayName("Chi nhánh")]
        public string? SpaName { get; set; }
        public List<BedViewModel> Beds { get; set; }
    }
}
