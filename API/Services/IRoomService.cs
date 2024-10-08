﻿using API.Dtos;
using API.Models;
using API.Ultils;

namespace API.Services
{
    public interface IRoomService
    {
        Task<Room> CreateRoom(Room room);
        Task<IEnumerable<Room>> GetRoomBySpaId(string spaId = null);
        Task<PaginatedList<RoomDTO>> GetRooms(int pageIndex, int pageSize, string searchTerm, string spaId);
        List<Room> GetAllRooms();
        Room GetRoom(int id);
        Task<Room> UpdateRoom(int id, Room room);
        Task<bool> DeleteRoom(int id);
        bool RoomExist(int id);
    }
}
