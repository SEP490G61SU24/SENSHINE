using API.Models;

namespace API.Services
{
    public interface IRoomService
    {
        Task<Room> CreateRoom(Room room);
        Task<IEnumerable<Room>> GetRoomBySpaId(int spaId); //lay room tu SpaId
        ICollection<Room> GetRooms();
        Room GetRoom(int id);
        Task<Room> UpdateRoom(int id, Room room);
        Task<bool> DeleteRoom(int id);
        bool RoomExist(int id);
    }
}
