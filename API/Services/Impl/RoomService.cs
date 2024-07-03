using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Impl
{
    public class RoomService : IRoomService
    {
        private readonly SenShineSpaContext _context;

        public RoomService(SenShineSpaContext context)
        {
            _context = context;
        }

        public async Task<Room> AddRoom(string roomName)
        {
            var room = new Room
            {
                RoomName = roomName
            };

            _context.Rooms.Add(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<Room> UpdateRoom(int id, string roomName)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return null;
            }

            room.RoomName = roomName;

            _context.Rooms.Update(room);
            await _context.SaveChangesAsync();
            return room;
        }

        public async Task<bool> DeleteRoom(int id)
        {
            var room = await _context.Rooms.FindAsync(id);
            if (room == null)
            {
                return false;
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<Room> GetRoomById(int id)
        {
            return await _context.Rooms.FindAsync(id);
        }

        public async Task<IEnumerable<Room>> GetAllRooms()
        {
            return await _context.Rooms.ToListAsync();
        }
    }
}
