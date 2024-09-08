namespace API.Dtos
{
    public class SlotDTO
    {
        public int Id { get; set; }
        public string SlotName { get; set; } = null!;
        public TimeSpan? TimeFrom { get; set; }
        public TimeSpan? TimeTo { get; set; }
    }
}
