namespace API.Dtos
{
	public class ChangePasswordDTO
	{
		public string? Token { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
		public string? OldPassword { get; set; }
		public string NewPassword { get; set; }
		public string? RePassword { get; set; }
	}
}
