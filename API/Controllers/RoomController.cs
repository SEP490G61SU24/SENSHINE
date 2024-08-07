using API.Dtos;
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

                return Ok($"Tạo room {createdRoom.RoomName} thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Có lỗi xảy ra khi tạo room: {ex.Message}");
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var rooms = _mapper.Map<List<RoomDTO>>(_roomService.GetRooms());

            return Ok(rooms);
        }

        [HttpGet]
        public async Task<IActionResult> GetById(int id)
        {
            if (!_roomService.RoomExist(id))
                return NotFound();

            var room = _mapper.Map<RoomDTO>(_roomService.GetRoom(id));

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            return Ok(room);
        }

        [HttpPut]
        public async Task<IActionResult> Update(int id, [FromBody] RoomDTO roomDTO)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!_roomService.RoomExist(id))
                return NotFound();

            try
            {
                var existingRoom = _roomService.GetRoom(id);
                existingRoom.RoomName = roomDTO.RoomName;
                existingRoom.SpaId = roomDTO.SpaId;
                var roomUpdate = await _roomService.UpdateRoom(id, existingRoom);

                if (roomUpdate == null)
                {
                    return NotFound("Không thể cập nhật room");
                }

                return Ok($"Cập nhật room {roomUpdate.RoomName} thành công");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Có lỗi xảy ra khi cập nhật room: {ex.Message}");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _roomService.DeleteRoom(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }
}
