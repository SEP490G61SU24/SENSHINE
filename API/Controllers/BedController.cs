using API.Dtos;
using API.Models;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{

    [Route("api/[controller]/[action]")]
    [ApiController]
    public class BedController : ControllerBase
    {
        private readonly IBedService _bedService;
        private readonly IMapper _mapper;

        public BedController(IBedService bedService, IMapper mapper)
        {
            _bedService = bedService;
            _mapper = mapper;
        }

        [HttpPost]
        [Route("/api/[controller]/[action]")]
        public async Task<IActionResult> AddBed(BedDTO bedDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var bed = _mapper.Map<Bed>(bedDTO);
                var addedBed = await _bedService.AddBed(bed);
                var addedBedDTO = _mapper.Map<BedDTO>(addedBed);

                return CreatedAtAction(nameof(GetBedById), new { id = addedBedDTO.Id }, addedBedDTO);
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


        [HttpPut]
        [Route("/api/[controller]/[action]/{id}")]
        public async Task<IActionResult> UpdateBed(int id, BedDTO bedDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var updatedBedDTO = await _bedService.UpdateBedAsync(id, bedDTO);
                if (updatedBedDTO == null)
                {
                    return NotFound();
                }

                return Ok(updatedBedDTO);
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
        public async Task<IActionResult> DeleteBed(int id)
        {
            try
            {
                var success = await _bedService.DeleteBed(id);
                if (!success)
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

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBedById(int id)
        {
            try
            {
                var bedDTO = await _bedService.GetBedById(id);
                if (bedDTO == null)
                {
                    return NotFound();
                }

                return Ok(bedDTO);
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

        [HttpGet]
        public async Task<IActionResult> GetAllBeds()
        {
            try
            {
                var bedDTOs = await _bedService.GetAllBeds();
                return Ok(bedDTOs);
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

        //lay ra danh sach phong theo RoomId
        [HttpGet("ByRoomId/{roomId}")]
        public async Task<IActionResult> GetByRoomId(int roomId)
        {
            try
            {
                var beds = await _bedService.GetBedByRoomId(roomId);
                if(beds == null)
                {
                    return NotFound();
                }
                return Ok(_mapper.Map<IEnumerable<BedDTO>>(beds));
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

        [HttpGet]
        public async Task<IActionResult> GetAllBedsPaging([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null)
        {
            try
            {
                if (pageIndex < 1 || pageSize < 1)
                {
                    return BadRequest("Chỉ số trang hoặc kích thước trang không hợp lệ.");
                }

                var pageData = await _bedService.GetBedList(pageIndex, pageSize, searchTerm);
                return Ok(pageData);
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
