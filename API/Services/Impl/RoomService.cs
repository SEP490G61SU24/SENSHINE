using API.Dtos;
using API.Models;
using API.Ultils;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;

namespace API.Services.Impl
{
    public class RoomService : IRoomService
    {
        private readonly SenShineSpaContext _context;
        private readonly IMapper _mapper;

        public RoomService(SenShineSpaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
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

        public List<Room> GetAllRooms()
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
                var rooms = GetAllRooms();
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

        public async Task<PaginatedList<RoomDTO>> GetRooms(int pageIndex, int pageSize, string searchTerm, string spaId)
        {
            // Tạo query cơ bản
            IQueryable<Room> query = _context.Rooms;
            int? spaIdInt = spaId != null && spaId != "ALL"
            ? int.Parse(spaId)
            : (int?)null;

            if (spaIdInt.HasValue)
            {
                query = query.Where(u => u.SpaId == spaIdInt.Value);
            }

            // Nếu có searchTerm, thêm điều kiện tìm kiếm vào query
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(r => r.RoomName.Contains(searchTerm));
            }

            // Đếm tổng số bản ghi để tính tổng số trang
            var count = await query.CountAsync();

            // Lấy danh sách với phân trang
            var rooms = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();
            var roomsDtos = _mapper.Map<IEnumerable<RoomDTO>>(rooms);

            return new PaginatedList<RoomDTO>
            {
                Items = roomsDtos,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = count,
            };
        }
    }
}
