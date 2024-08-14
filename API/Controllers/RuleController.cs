using AutoMapper;
using API.Dtos;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/rules")]
    public class RuleController : ControllerBase
    {
        private readonly IRuleService _ruleService;
        private readonly IMapper _mapper;

        public RuleController(IRuleService ruleService, IMapper mapper)
        {
            _ruleService = ruleService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
        {
            try
            {
                if (pageIndex < 1 || pageSize < 1)
                {
                    return BadRequest("Chỉ số trang hoặc kích thước trang không hợp lệ.");
                }

                var paged = await _ruleService.GetRules(pageIndex, pageSize, searchTerm);
                return Ok(paged);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var rule = await _ruleService.GetRuleById(id);
            if (rule == null)
            {
                return NotFound();
            }

            return Ok(rule);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RuleDTO ruleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdRule = await _ruleService.AddRule(ruleDto);
            return CreatedAtAction(nameof(GetById), new { id = createdRule.Id }, createdRule);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] RuleDTO ruleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _ruleService.UpdateRule(id, ruleDto);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _ruleService.DeleteRule(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }

		[HttpGet("role/{roleId}")]
		public async Task<IActionResult> GetRulesByRoleId(int roleId)
		{
			var rules = await _ruleService.GetRulesByRoleId(roleId);
			if (rules == null || !rules.Any())
			{
				return NotFound("No rules found for this role.");
			}
			return Ok(rules);
		}
	}
}
