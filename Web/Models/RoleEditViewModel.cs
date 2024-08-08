using API.Dtos;

namespace Web.Models
{
	public class RoleEditViewModel
	{
		public int RoleId { get; set; }
        public RoleDTO Role { get; set; }
		public IEnumerable<RuleDTO> AllRules { get; set; }
        public IEnumerable<RuleDTO> RoleRules { get; set; }
        public IEnumerable<int> SelectedRuleIds { get; set; }
    }
}
