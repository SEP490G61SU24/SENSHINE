﻿using API.Dtos;
using API.Models;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    public class RoomController : Controller
    {
        private readonly IRoomService _roomService;
        private readonly IMapper _mapper;

        public RoomController(IRoomService roomService, IMapper mapper)
        {
            _roomService = roomService;
            _mapper = mapper;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] RoomDTO roomDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var roomMap = _mapper.Map<Room>(roomDTO);
                var createdRoom = await _roomService.CreateRoom(roomMap);

                return Ok($"Room {createdRoom.RoomName} created successfully.");
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
        public async Task<IActionResult> GetAll([FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null, [FromQuery] string? spaId = null)
        {
            try
            {
                if (pageIndex < 1 || pageSize < 1)
                {
                    return BadRequest("Chỉ số trang hoặc kích thước trang không hợp lệ.");
                }

                var rooms = await _roomService.GetRooms(pageIndex, pageSize, searchTerm, spaId);
                return Ok(rooms);
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
        public async Task<IActionResult> GetAllRoom()
        {
            try
            {

                var rooms = _roomService.GetAllRooms();
                return Ok(rooms);
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
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                if (!_roomService.RoomExist(id))
                    return NotFound("Room not found.");

                var room = _mapper.Map<RoomDTO>(_roomService.GetRoom(id));
                return Ok(room);
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
        public async Task<IActionResult> Update(int id, [FromBody] RoomDTO roomDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                if (!_roomService.RoomExist(id))
                    return NotFound("Room not found.");

                var existingRoom = _roomService.GetRoom(id);
                existingRoom.RoomName = roomDTO.RoomName;
                existingRoom.SpaId = roomDTO.SpaId;
                var roomUpdate = await _roomService.UpdateRoom(id, existingRoom);

                if (roomUpdate == null)
                {
                    return NotFound("Room could not be updated.");
                }

                return Ok($"Room {roomUpdate.RoomName} updated successfully.");
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

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var result = await _roomService.DeleteRoom(id);
                if (!result)
                {
                    return NotFound("Room not found.");
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

        [HttpGet]
        public async Task<IActionResult> GetBySpaId(int spaId)
        {
            try
            {
                var rooms = await _roomService.GetRoomBySpaId(spaId);
                if (rooms == null || !rooms.Any())
                {
                    return NotFound("No rooms found for the specified Spa.");
                }
                return Ok(_mapper.Map<IEnumerable<RoomDTO>>(rooms));
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
