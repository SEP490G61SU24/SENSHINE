using AutoMapper;
using API.Dtos;
using API.Models;
using Microsoft.EntityFrameworkCore;

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

        public async Task<RuleDTO> GetRuleById(int id)
        {
            var rule = await _context.Rules.FindAsync(id);
            return _mapper.Map<RuleDTO>(rule);
        }

        public async Task<RuleDTO> AddRule(RuleDTO ruleDto)
        {
            var rule = _mapper.Map<Rule>(ruleDto);
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
	}
}
