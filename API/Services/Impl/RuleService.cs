using AutoMapper;
using API.Dtos;
using API.Models;
using Microsoft.EntityFrameworkCore;
using API.Ultils;
using System.Data;

namespace API.Services.Impl
{
    public class RuleService : IRuleService
    {
        private readonly SenShineSpaContext _context;
        private readonly IMapper _mapper;

        public RuleService(SenShineSpaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RuleDTO>> GetAllRules()
        {
            var rules = await _context.Rules.ToListAsync();
            return _mapper.Map<IEnumerable<RuleDTO>>(rules);
        }

        public async Task<PaginatedList<RuleDTO>> GetRules(int pageIndex, int pageSize, string searchTerm)
        {
            // Tạo query cơ bản
            IQueryable<API.Models.Rule> query = _context.Rules;

            // Nếu có searchTerm, thêm điều kiện tìm kiếm vào query
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r => r.Title.Contains(searchTerm) ||
                                         r.Path.Contains(searchTerm) ||
                                         r.Remark.Contains(searchTerm));
            }

            // Đếm tổng số bản ghi để tính tổng số trang
            var count = await query.CountAsync();

            // Lấy danh sách với phân trang
            var dataRules = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();

            var ruleDtos = _mapper.Map<IEnumerable<RuleDTO>>(dataRules);

            return new PaginatedList<RuleDTO>
            {
                Items = ruleDtos,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = count,
            };
        }

        public async Task<RuleDTO> GetRuleById(int id)
        {
            var rule = await _context.Rules.FindAsync(id);
            return _mapper.Map<RuleDTO>(rule);
        }

        public async Task<RuleDTO> AddRule(RuleDTO ruleDto)
        {
            var rule = _mapper.Map<API.Models.Rule>(ruleDto);
            _context.Rules.Add(rule);
            await _context.SaveChangesAsync();
            ruleDto.Id = rule.Id;
            return ruleDto;
        }

        public async Task<bool> UpdateRule(int id, RuleDTO ruleDto)
        {
            var rule = await _context.Rules.FindAsync(id);
            if (rule == null)
            {
                return false;
            }

            _mapper.Map(ruleDto, rule);
            _context.Rules.Update(rule);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteRule(int id)
        {
            var rule = await _context.Rules.FindAsync(id);
            if (rule == null)
            {
                return false;
            }

            _context.Rules.Remove(rule);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<RuleDTO>> GetRulesByRoleId(int roleId)
        {
            var role = await _context.Roles
                .Where(r => r.Id == roleId)
                .Select(r => new
                {
                    r.Id,
                    r.Rules
                })
                .FirstOrDefaultAsync();

            if (role == null)
            {
                return Enumerable.Empty<RuleDTO>();
            }

            var ruleIdStrings = role.Rules.Split(',');

            var ruleIds = ruleIdStrings
                .Select(id => int.TryParse(id.Trim(), out var parsedId) ? parsedId : (int?)null)
                .Where(id => id.HasValue)
                .Select(id => id.Value)
                .ToList();

            var rules = await _context.Rules
                .Where(r => ruleIds.Contains(r.Id))
                .ToListAsync();

            return _mapper.Map<IEnumerable<RuleDTO>>(rules);
        }

        public async Task<IEnumerable<RuleDTO>> ExcludeId(int id)
        {
            var rules = await _context.Rules.Where(r => r.Id != id).ToListAsync();
            return _mapper.Map<IEnumerable<RuleDTO>>(rules);
        }

        public async Task<IEnumerable<MenuDTO>> GetMenuItemsByRoleAsync(int roleId)
        {
            Role role = await _context.Roles.FirstOrDefaultAsync(r => r.Id == roleId);

            if (role == null)
            {
                return Enumerable.Empty<MenuDTO>();
            }

            var ruleIds = role.Rules.Split(',').Select(int.Parse).ToList();

            var rules = await _context.Rules
                .Where(r => ruleIds.Contains(r.Id) && r.Ismenu == true)
                .OrderBy(r => r.Order)
                .ToListAsync();

            IEnumerable<RuleDTO> rulesDto = _mapper.Map<IEnumerable<RuleDTO>>(rules);

            var menuItems = rulesDto
                .Where(r => r.Pid == 0)
                .Select(r => new MenuDTO
                {
                    Menu = r,
                    Children = GetChildren(r.Id, rulesDto)
                });

            return menuItems;
        }

        private IEnumerable<MenuDTO> GetChildren(int parentId, IEnumerable<RuleDTO> allRules)
        {
            return allRules
                .Where(r => r.Pid == parentId)
                .Select(r => new MenuDTO
                {
                    Menu = r,
                    Children = GetChildren(r.Id, allRules)
                });
        }

        public async Task<bool> CheckAccessAsync(int? roleId, string path)
        {
            if (roleId == null)
            {
                return false;
            }

            // Lấy các quyền của vai trò
            var rules = await GetRulesByRoleId(roleId.Value);

            // Kiểm tra xem path có nằm trong danh sách quyền của vai trò không
            return rules.Any(rule => rule.Path.Equals(path, StringComparison.OrdinalIgnoreCase));
        }
    }
}
