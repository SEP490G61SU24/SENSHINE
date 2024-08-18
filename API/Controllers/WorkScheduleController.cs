using AutoMapper;
using API.Dtos;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using API.Services.Impl;
using System.Globalization;

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
        public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
        {
            try
            {
                if (pageIndex < 1 || pageSize < 1)
                {
                    return BadRequest("Chỉ số trang hoặc kích thước trang không hợp lệ.");
                }

                var paged = await _workScheduleService.GetWorkSchedules(pageIndex, pageSize, searchTerm);
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

		[HttpGet("weeks")]
		public async Task<IActionResult> GetAvailableWeeks([FromQuery] string employeeId)
		{
			try
			{
				if (string.IsNullOrEmpty(employeeId))
				{
					return BadRequest("ID nhân viên bắt buộc!");
				}

				var weeks = await _workScheduleService.GetAvailableWeeks(int.Parse(employeeId));
				return Ok(weeks);
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

		[HttpGet("current-user")]
        public async Task<IActionResult> GetByCurrentUser([FromQuery] string employeeId, [FromQuery] int year, [FromQuery] int weekNumber)
        {
			try
			{
				if (string.IsNullOrEmpty(employeeId))
				{
					return BadRequest("ID nhân viên bắt buộc!");
				}

				var startDate = ISOWeek.ToDateTime(year, weekNumber, DayOfWeek.Monday);
				var endDate = ISOWeek.ToDateTime(year, weekNumber, DayOfWeek.Sunday);

				var workSchedules = await _workScheduleService.GetWorkSchedulesByWeek(int.Parse(employeeId), startDate, endDate);
				return Ok(workSchedules);
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
