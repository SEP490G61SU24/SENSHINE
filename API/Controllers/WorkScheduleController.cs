using AutoMapper;
using API.Dtos;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/work-schedules")]
    public class WorkScheduleController : ControllerBase
    {
        private readonly IWorkScheduleService _workScheduleService;
        private readonly IMapper _mapper;

        public WorkScheduleController(IWorkScheduleService workScheduleService, IMapper mapper)
        {
            _workScheduleService = workScheduleService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var workSchedules = await _workScheduleService.GetAllWorkSchedules();
            return Ok(workSchedules);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var workScheduleDto = await _workScheduleService.GetWorkScheduleById(id);
            if (workScheduleDto == null)
            {
                return NotFound();
            }

            return Ok(workScheduleDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WorkScheduleDTO workScheduleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var workSchedule = await _workScheduleService.AddWorkSchedule(workScheduleDto);
            return CreatedAtAction(nameof(GetById), new { id = workSchedule.Id }, workSchedule);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] WorkScheduleDTO workScheduleDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedWorkSchedule = await _workScheduleService.UpdateWorkSchedule(id, workScheduleDto);
            if (updatedWorkSchedule == null)
            {
                return NotFound();
            }

            return Ok(updatedWorkSchedule);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _workScheduleService.DeleteWorkSchedule(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
