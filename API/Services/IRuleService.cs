using API.Dtos;

namespace API.Services
{
    public interface IRuleService
    {
        Task<IEnumerable<RuleDTO>> GetAllRules();
        Task<RuleDTO> GetRuleById(int id);
        Task<RuleDTO> AddRule(RuleDTO ruleDto);
        Task<bool> UpdateRule(int id, RuleDTO ruleDto);
        Task<bool> DeleteRule(int id);
        Task<IEnumerable<RuleDTO>> GetRulesByRoleId(int roleId);
	}
}
