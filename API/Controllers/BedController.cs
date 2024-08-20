using API.Dtos;
using API.Models;
using API.Services;
using API.Services.Impl;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

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
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message); // Return a 400 status code with the error message
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
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBed(int id)
        {
            var success = await _bedService.DeleteBed(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetBedById(int id)
        {
            var bedDTO = await _bedService.GetBedById(id);
            if (bedDTO == null)
            {
                return NotFound();
            }

            return Ok(bedDTO);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBeds()
        {
            var bedDTOs = await _bedService.GetAllBeds();
            return Ok(bedDTOs);
        }

        //lay ra danh sach phong theo RoomId
        [HttpGet("ByRoomId/{roomId}")]
        public async Task<IActionResult> GetByRoomId(int roomId)
        {
            var beds = await _bedService.GetBedByRoomId(roomId);
            if(beds == null)
            {
                return NotFound();
            }
            return Ok(_mapper.Map<IEnumerable<BedDTO>>(beds));
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
