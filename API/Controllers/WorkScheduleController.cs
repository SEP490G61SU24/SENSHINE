using API.Dtos;
using API.Services;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;

namespace API.Controllers
{
    [ApiController]
    [Route("api/work-schedules")]
    public class WorkScheduleController : ControllerBase
    {
        private readonly IWorkScheduleService _workScheduleService;

        public WorkScheduleController(IWorkScheduleService workScheduleService)
        {
            _workScheduleService = workScheduleService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null, [FromQuery] string? spaId = null)
        {
            try
            {
                if (pageIndex < 1 || pageSize < 1)
                {
                    return BadRequest("Chỉ số trang hoặc kích thước trang không hợp lệ.");
                }

                var paged = await _workScheduleService.GetWorkSchedules(pageIndex, pageSize, searchTerm, spaId);
                return Ok(paged);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
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

		[HttpGet("years")]
		public async Task<IActionResult> GetAvailableYears([FromQuery] string employeeId)
		{
			try
			{
				if (string.IsNullOrEmpty(employeeId))
				{
					return BadRequest("ID nhân viên bắt buộc!");
				}

				var years = await _workScheduleService.GetAvailableYears(int.Parse(employeeId));
				return Ok(years);
			}
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
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
		public async Task<IActionResult> GetAvailableWeeks([FromQuery] string employeeId, [FromQuery] string year)
		{
			try
			{
				if (string.IsNullOrEmpty(employeeId))
				{
					return BadRequest("ID nhân viên bắt buộc!");
				}

				var weeks = await _workScheduleService.GetAvailableWeeks(int.Parse(employeeId), int.Parse(year));
				return Ok(weeks);
			}
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
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

				// Xác định múi giờ Việt Nam
				var vietnamTimeZone = TimeZoneInfo.FindSystemTimeZoneById("SE Asia Standard Time");

				// Lấy ngày bắt đầu và kết thúc của tuần trong múi giờ UTC
				var startDateUtc = ISOWeek.ToDateTime(year, weekNumber, DayOfWeek.Monday);
				var endDateUtc = ISOWeek.ToDateTime(year, weekNumber, DayOfWeek.Sunday);

				// Chuyển đổi từ UTC sang múi giờ Việt Nam
				var startDate = TimeZoneInfo.ConvertTimeFromUtc(startDateUtc, vietnamTimeZone);
				var endDate = TimeZoneInfo.ConvertTimeFromUtc(endDateUtc, vietnamTimeZone);

				var workSchedules = await _workScheduleService.GetWorkSchedulesByWeek(int.Parse(employeeId), startDate, endDate);
				return Ok(workSchedules);
			}
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
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
            try
            {
                var workScheduleDto = await _workScheduleService.GetWorkScheduleById(id);
                if (workScheduleDto == null)
                {
                    return NotFound();
                }

                return Ok(workScheduleDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
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

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WorkScheduleDTO workScheduleDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var workSchedule = await _workScheduleService.AddWorkSchedule(workScheduleDto);
                return CreatedAtAction(nameof(GetById), new { id = workSchedule.Id }, workSchedule);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] WorkScheduleDTO workScheduleDto)
        {
            try
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
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _workScheduleService.DeleteWorkSchedule(id);
                if (!result)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
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
    }
}
