using API.Dtos;

namespace Web.Models
{
    public class CurrentWorkScheduleViewModel
    {
        public IEnumerable<WeekOptionDTO> AvailableWeeks { get; set; }
        public int SelectedWeek { get; set; }
        public IEnumerable<WorkScheduleDTO> WorkSchedules { get; set; }
        public List<string> TimeSlots { get; set; } = new List<string>
        {
			"07:00 - 08:00 AM", "08:00 - 09:00 AM", "09:00 - 10:00 AM", "10:00 - 11:00 AM",
			 "02:00 - 03:00 PM", "03:00 - 04:00 PM", "04:00 - 05:00 PM", "05:00 - 06:00 PM"
		};
    }
}
