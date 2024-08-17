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
            try
            {
                await _context.Rooms.AddAsync(room);
                await _context.SaveChangesAsync();

                return room;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("Error creating room.", ex);
            }
        }

        public ICollection<Room> GetRooms()
        {
            try
            {
                return _context.Rooms.ToList();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("Error retrieving rooms.", ex);
            }
        }

        public Room GetRoom(int id)
        {
            try
            {
                var rooms = GetRooms();
                return rooms.FirstOrDefault(r => r.Id == id);
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception($"Error retrieving room with ID {id}.", ex);
            }
        }

        public async Task<Room> UpdateRoom(int id, Room room)
        {
            try
            {
                var roomUpdate = await _context.Rooms.FirstOrDefaultAsync(r => r.Id == id);
                if (roomUpdate != null)
                {
                    roomUpdate.RoomName = room.RoomName;
                    roomUpdate.SpaId = room.SpaId;
                    await _context.SaveChangesAsync();
                }

                return roomUpdate;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception($"Error updating room with ID {id}.", ex);
            }
        }

        public async Task<bool> DeleteRoom(int id)
        {
            try
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
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception($"Error deleting room with ID {id}.", ex);
            }
        }

        public bool RoomExist(int id)
        {
            try
            {
                return _context.Rooms.Any(s => s.Id == id);
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception($"Error checking existence of room with ID {id}.", ex);
            }
        }

        public async Task<IEnumerable<Room>> GetRoomBySpaId(int spaId)
        {
            try
            {
                return await _context.Rooms.Include(r => r.Spa)
                                           .Where(r => r.Spa.Id == spaId)
                                           .ToListAsync();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception($"Error retrieving rooms for Spa ID {spaId}.", ex);
            }
        }
    }
}
