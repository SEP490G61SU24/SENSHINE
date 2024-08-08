using AutoMapper;
using API.Dtos;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Impl
{
    public class RoleService : IRoleService
    {
        private readonly SenShineSpaContext _context;
        private readonly IMapper _mapper;

        public RoleService(SenShineSpaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RoleDTO>> GetAllRoles()
        {
            var roles = await _context.Roles.ToListAsync();
            return _mapper.Map<IEnumerable<RoleDTO>>(roles);
        }

        public async Task<RoleDTO> GetRoleById(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            return _mapper.Map<RoleDTO>(role);
        }

        public async Task<RoleDTO> AddRole(RoleDTO roleDto)
        {
            var role = _mapper.Map<Role>(roleDto);
            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
            roleDto.Id = role.Id;
            return roleDto;
        }

        public async Task<bool> UpdateRole(int id, RoleDTO roleDto)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return false;
            }

            _mapper.Map(roleDto, role);
            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRole(int id)
        {
            var role = await _context.Roles.FindAsync(id);
            if (role == null)
            {
                return false;
            }

            _context.Roles.Remove(role);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateRoleRules(int roleId, IEnumerable<int> ruleIds)
        {
            var role = await _context.Roles.FindAsync(roleId);
            if (role == null)
            {
                return false;
            }

            role.Rules = string.Join(',', ruleIds);

            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
