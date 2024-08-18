using API.Dtos;

namespace Web.Models
{
    public class RuleEditViewModel
    {
        public RuleDTO ruleDTO { get; set; }
        public IEnumerable<RuleDTO> parentRulesDTO { get; set; }
        public IEnumerable<string> iconList { get; set; }

	}
}
