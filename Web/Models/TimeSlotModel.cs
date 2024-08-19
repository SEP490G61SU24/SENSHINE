namespace Web.Models
{
	public class TimeSlotModel
	{
		public TimeSpan StartTime { get; set; }
		public TimeSpan EndTime { get; set; }
		public string Period { get; set; } // "Sáng", "Trưa", "Chiều", "Tối"
	}
}
