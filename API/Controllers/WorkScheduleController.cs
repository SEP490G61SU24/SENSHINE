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
        private const string ErrorMessage = "An error occurred: ";
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

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                await _workScheduleService.UpdateWorkSchedulesStatus(id);

                return Ok();
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

        [HttpPost("add-work-employee")]
        public async Task<IActionResult> AddWorkEmployee(int employeeId, int slotId, DateTime date)
        {
            try
            {
                // Ensure all required fields are valid
                if (employeeId <= 0 || slotId <= 0 || date == default)
                {
                    return BadRequest("Invalid parameters.");
                }

                var userSlot = await _workScheduleService.AddWorkThisEmployee(employeeId, slotId, date);
                if (userSlot == null)
                {
                    return NotFound("User work could not be added.");
                }

                return Ok($"Add work for employee {employeeId} successfully for slot ID: {slotId} on {date:yyyy-MM-dd}");
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
                return StatusCode(StatusCodes.Status500InternalServerError, ErrorMessage + ex.Message);
            }
        }

    }
}
