using API.Models;

namespace API.Services
{
    public interface IRoomService
    {
        Task<Room> AddRoom(string roomName);
        Task<Room> UpdateRoom(int id, string roomName);
        Task<bool> DeleteRoom(int id);
        Task<Room> GetRoomById(int id);
        Task<IEnumerable<Room>> GetAllRooms();
    }
}
