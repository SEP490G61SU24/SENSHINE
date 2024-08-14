using API.Dtos;

namespace Web.Models
{
	public class WorkScheduleViewModel
	{
		public int Id { get; set; }
		public IEnumerable<UserDTO>? Employees { get; set; }
		public WorkScheduleDTO? WorkScheduleData { get; set; }
		public DateTime? StartDateTime { get; set; }
		public DateTime? EndDateTime { get; set;}
		public int? EmployeeId { get; set; }
		public string Status { get; set; }
	}
}
