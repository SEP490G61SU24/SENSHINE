using API.Dtos;

namespace API.Services
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDTO>> GetAllRoles();
        Task<RoleDTO> GetRoleById(int id);
        Task<RoleDTO> AddRole(RoleDTO roleDto);
        Task<bool> UpdateRole(int id, RoleDTO roleDto);
        Task<bool> DeleteRole(int id);
        Task<bool> UpdateRoleRules(int id, IEnumerable<int> ruleIds);
    }
}
