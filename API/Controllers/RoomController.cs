using API.Models;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [ApiController]
    [Route("api/rooms")]
    public class RoomController : ControllerBase
    {
        private readonly IRoomService _roomService;

        public RoomController(IRoomService roomService)
        {
            _roomService = roomService;
        }

        [HttpPost]
        public async Task<IActionResult> AddRoom(Room room)
        {
            var addedRoom = await _roomService.AddRoom(room.RoomName);
            return CreatedAtAction(nameof(GetRoomById), new { id = addedRoom.Id }, addedRoom);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRoom(int id, Room room)
        {
            var updatedRoom = await _roomService.UpdateRoom(id, room.RoomName);
            if (updatedRoom == null)
            {
                return NotFound();
            }

            return Ok(updatedRoom);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRoom(int id)
        {
            var success = await _roomService.DeleteRoom(id);
            if (!success)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetRoomById(int id)
        {
            var room = await _roomService.GetRoomById(id);
            if (room == null)
            {
                return NotFound();
            }

            return Ok(room);
        }

        [HttpGet]
        public async Task<IActionResult> GetAllRooms()
        {
            var rooms = await _roomService.GetAllRooms();
            return Ok(rooms);
        }
    }
}
