using System.ComponentModel;

namespace Web.Models
{
    public class ServiceViewModel
    {
        [DisplayName("ID")]
        public int Id { get; set; }

        [DisplayName("Tên Dịch Vụ")]
        public string ServiceName { get; set; } = null!;

        [DisplayName("Giá Tiền")]
        public decimal Amount { get; set; }

        [DisplayName("Mô Tả Dịch Vụ")]
        public string? Description { get; set; }
    }
}
