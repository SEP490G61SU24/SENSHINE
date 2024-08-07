﻿using AutoMapper;
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
        public async Task<IActionResult> GetAll()
        {
            var rules = await _ruleService.GetAllRules();
            return Ok(rules);
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
    }
}
