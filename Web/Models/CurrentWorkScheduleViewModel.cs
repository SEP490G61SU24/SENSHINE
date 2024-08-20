using API.Dtos;

namespace Web.Models
{
    public class CurrentWorkScheduleViewModel
    {
		public string? UserString { get; set; }
        public IEnumerable<int> AvailableYears { get; set; }
        public int SelectedYear { get; set; }
        public IEnumerable<WeekOptionDTO> AvailableWeeks { get; set; }
        public int SelectedWeek { get; set; }
        public IEnumerable<WorkScheduleDTO> WorkSchedules { get; set; }
		public Dictionary<string, TimeSlotModel> TimeSlots { get; set; } = new Dictionary<string, TimeSlotModel>
		{
			{ "Slot 1", new TimeSlotModel { StartTime = new TimeSpan(8, 30, 0), EndTime = new TimeSpan(10, 0, 0), Period = "Sáng" } },
			{ "Slot 2", new TimeSlotModel { StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(11, 30, 0), Period = "Trưa" } },
			{ "Slot 3", new TimeSlotModel { StartTime = new TimeSpan(11, 30, 0), EndTime = new TimeSpan(13, 0, 0), Period = "Trưa" } },
			{ "Slot 4", new TimeSlotModel { StartTime = new TimeSpan(13, 0, 0), EndTime = new TimeSpan(14, 30, 0), Period = "Chiều" } },
			{ "Slot 5", new TimeSlotModel { StartTime = new TimeSpan(14, 30, 0), EndTime = new TimeSpan(16, 0, 0), Period = "Chiều" } },
			{ "Slot 6", new TimeSlotModel { StartTime = new TimeSpan(16, 0, 0), EndTime = new TimeSpan(17, 30, 0), Period = "Chiều" } },
			{ "Slot 7", new TimeSlotModel { StartTime = new TimeSpan(17, 30, 0), EndTime = new TimeSpan(19, 0, 0), Period = "Tối" } },
			{ "Slot 8", new TimeSlotModel { StartTime = new TimeSpan(19, 0, 0), EndTime = new TimeSpan(20, 30, 0), Period = "Tối" } },
			{ "Slot 9", new TimeSlotModel { StartTime = new TimeSpan(20, 30, 0), EndTime = new TimeSpan(22, 0, 0), Period = "Tối" } }
		};
	}
}
