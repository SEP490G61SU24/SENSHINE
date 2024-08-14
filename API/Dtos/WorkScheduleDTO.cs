using System.ComponentModel.DataAnnotations;

namespace API.Dtos
{
    public class WorkScheduleDTO
    {
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập giờ bắt đầu.")]
		public DateTime? StartDateTime { get; set; }
		[Required(ErrorMessage = "Vui lòng nhập giờ kết thúc.")]
		public DateTime? EndDateTime { get; set; }
        public string? Status { get; set; }
        public string? EmployeeName { get; set; }
    }
}
