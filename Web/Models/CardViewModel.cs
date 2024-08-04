using System.ComponentModel;

namespace Web.Models
{
    public class CardViewModel
    {
        public int Id { get; set; }
        [DisplayName("Mã thẻ")]
        public string CardNumber { get; set; } = null!;
        public int CustomerId { get; set; }
        [DisplayName("Ngày tạo")]
        public DateTime? CreateDate { get; set; }
        [DisplayName("Trạng thái")]
        public string? Status { get; set; }
        public int? BranchId { get; set; }
        [DisplayName("Khách hàng")]
        public string? CustomerName { get; set; }
        [DisplayName("Chi nhánh")]
        public string? BranchName { get; set; }
        [DisplayName("Số điện thoại")]
        public string? CustomerPhone { get; set; }
    }
}