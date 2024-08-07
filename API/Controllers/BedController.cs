﻿using API.Dtos;
using API.Models;
using API.Services;
using API.Services.Impl;
using AutoMapper;
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

            var bed = _mapper.Map<Bed>(bedDTO);
            var addedBed = await _bedService.AddBed(bed);
            var addedBedDTO = _mapper.Map<BedDTO>(addedBed);

            return CreatedAtAction(nameof(GetBedById), new { id = addedBedDTO.Id }, addedBedDTO);
        }

        [HttpPut]
        [Route("/api/[controller]/[action]/{id}")]
        public async Task<IActionResult> UpdateBed(int id, BedDTO bedDTO)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedBed = await _bedService.UpdateBed(id, bedDTO.BedNumber);
            if (updatedBed == null)
            {
                return NotFound();
            }

            var updatedBedDTO = _mapper.Map<BedDTO>(updatedBed);
            return Ok(updatedBedDTO);
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

    }
}
