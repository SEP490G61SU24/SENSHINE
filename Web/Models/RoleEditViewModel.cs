using API.Dtos;

namespace Web.Models
{
	public class RoleEditViewModel
	{
		public int RoleId { get; set; }
        public RoleDTO Role { get; set; } = new RoleDTO();
		public IEnumerable<RuleDTO> AllRules { get; set; } = Enumerable.Empty<RuleDTO>();
        public IEnumerable<RuleDTO> RoleRules { get; set; } = Enumerable.Empty<RuleDTO>();
        public IEnumerable<int> SelectedRuleIds { get; set; } = Enumerable.Empty<int>();
    }
}
