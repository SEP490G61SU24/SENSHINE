using API.Dtos;
using API.Models;
using API.Services;
using API.Services.Impl;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/beds")]
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
        public async Task<IActionResult> AddBed(Bed bed)
        {
            var addedBed = await _bedService.AddBed(bed.BedNumber);
            return CreatedAtAction(nameof(GetBedById), new { id = addedBed.Id }, addedBed);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateBed(int id, Bed bed)
        {
            var updatedBed = await _bedService.UpdateBed(id, bed.BedNumber);
            if (updatedBed == null)
            {
                return NotFound();
            }

            return Ok(updatedBed);
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
            var bed = await _bedService.GetBedById(id);
            if (bed == null)
            {
                return NotFound();
            }

            return Ok(bed);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllBeds()
        {
            var beds = await _bedService.GetAllBeds();
            return Ok(beds);
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

    }
}
