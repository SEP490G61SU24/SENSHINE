namespace Web.Models
{
    public class SettingViewModel
    {
		public int Id { get; set; }
		public string Key { get; set; } = null!;
		public string Value { get; set; } = null!;
		public string? Description { get; set; }
	}
}
