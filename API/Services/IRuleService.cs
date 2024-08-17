using API.Dtos;
using API.Ultils;

namespace API.Services
{
    public interface IRuleService
    {
        Task<IEnumerable<RuleDTO>> GetAllRules();
        Task<PaginatedList<RuleDTO>> GetRules(int pageIndex, int pageSize, string searchTerm);
        Task<RuleDTO> GetRuleById(int id);
        Task<RuleDTO> AddRule(RuleDTO ruleDto);
        Task<bool> UpdateRule(int id, RuleDTO ruleDto);
        Task<bool> DeleteRule(int id);
        Task<IEnumerable<RuleDTO>> GetRulesByRoleId(int roleId);
        Task<IEnumerable<RuleDTO>> ExcludeId(int id);
        Task<IEnumerable<MenuDTO>> GetMenuItemsByRoleAsync(int roleId);
    }
}
