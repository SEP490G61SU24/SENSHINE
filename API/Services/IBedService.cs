using API.Dtos;
using API.Models;
using API.Ultils;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IBedService
    {
        Task<Bed> AddBed(Bed bed);
        Task<IEnumerable<Bed>> GetBedByRoomId(int roomId);
        Task<BedDTO> UpdateBedAsync(int id, BedDTO bedDTO);
        Task<bool> DeleteBed(int id);
        Task<BedDTO> GetBedById(int id); // Thay đổi trả về BedDTO
        Task<IEnumerable<BedDTO>> GetAllBeds(); // Thay đổi trả về BedDTO
        Task<PaginatedList<BedDTO>> GetBedList(int pageIndex = 1, int pageSize = 10, string? searchTerm = null);
    }
}
