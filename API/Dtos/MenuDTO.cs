namespace API.Dtos
{
    public class MenuDTO
    {
        public RuleDTO Menu {  get; set; }
        public IEnumerable<MenuDTO> Children { get; set; }
    }
}
