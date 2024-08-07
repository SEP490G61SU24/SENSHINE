namespace API.Dtos
{
    public class RuleDTO
    {
        public int Id { get; set; }
        public string Type { get; set; } = null!;
        public int Pid { get; set; }
        public string Path { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string? Icon { get; set; }
        public string? Url { get; set; }
        public string? Condition { get; set; }
        public string? Remark { get; set; }
        public bool? Ismenu { get; set; }
        public int Order { get; set; }
    }
}
