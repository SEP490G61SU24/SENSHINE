using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/workschedules")]
    public class WorkScheduleController : ControllerBase
    {
        private readonly IWorkScheduleService _workScheduleService;

        public WorkScheduleController(IWorkScheduleService workScheduleService)
        {
            _workScheduleService = workScheduleService;
        }

        [HttpPost]
        public async Task<IActionResult> AddWorkSchedule(WorkSchedule workSchedule)
        {
            var addedWorkSchedule = await _workScheduleService.AddWorkSchedule(workSchedule.EmployeeId, workSchedule.StartDateTime, workSchedule.EndDateTime, workSchedule.DayOfWeek);
            return CreatedAtAction(nameof(GetWorkScheduleById), new { id = addedWorkSchedule.Id }, addedWorkSchedule);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWorkSchedule(int id, WorkSchedule workSchedule)
        {
            var updatedWorkSchedule = await _workScheduleService.UpdateWorkSchedule(id, workSchedule.EmployeeId, workSchedule.StartDateTime, workSchedule.EndDateTime, workSchedule.DayOfWeek);
            if (updatedWorkSchedule == null)
            {
                return NotFound();
            }

            return Ok(updatedWorkSchedule);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWorkSchedule(int id)
        {
            var success = await _workScheduleService.DeleteWorkSchedule(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetWorkScheduleById(int id)
        {
            var workSchedule = await _workScheduleService.GetWorkScheduleById(id);
            if (workSchedule == null)
            {
                return NotFound();
            }

            return Ok(workSchedule);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllWorkSchedules()
        {
            var workSchedules = await _workScheduleService.GetAllWorkSchedules();
            return Ok(workSchedules);
        }
    }
}
