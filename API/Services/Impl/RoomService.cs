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

        public async Task<Room> CreateRoom(Room room)
        {
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();

            return room;
        }

        public ICollection<Room> GetRooms()
        {
            return _context.Rooms.ToList();
        }

        public Room GetRoom(int id)
        {
            var rooms = GetRooms();
            return rooms.FirstOrDefault(r => r.Id == id);
        }

        public async Task<Room> UpdateRoom(int id, Room room)
        {
            var roomUpdate = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
            roomUpdate.RoomName = room.RoomName;
            roomUpdate.SpaId = room.SpaId;
            await _context.SaveChangesAsync();

            return roomUpdate;
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

        public bool RoomExist(int id)
        {
            return _context.Rooms.Any(s => s.Id == id);
        }

        public async Task<IEnumerable<Room>> GetRoomBySpaId(int spaId)
        {
             return await _context.Rooms.Include(r => r.Spa)
                                     .Where(r => r.Spa.Id == spaId)
                                     .ToListAsync();
        }
    }
}
