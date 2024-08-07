namespace API.Dtos
{
    public class WorkScheduleDTO
    {
        public int Id { get; set; }
        public int? EmployeeId { get; set; }
        public DateTime? StartDateTime { get; set; }
        public DateTime? EndDateTime { get; set; }
        public string? DayOfWeek { get; set; }
        public string? EmployeeName { get; set; }
    }
}
