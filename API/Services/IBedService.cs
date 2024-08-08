using API.Dtos;
using API.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace API.Services
{
    public interface IBedService
    {
        Task<Bed> AddBed(Bed bed);
        Task<IEnumerable<Bed>> GetBedByRoomId(int roomId); 
        Task<Bed> UpdateBed(int id, string bedNumber);
        Task<bool> DeleteBed(int id);
        Task<BedDTO> GetBedById(int id); // Thay đổi trả về BedDTO
        Task<IEnumerable<BedDTO>> GetAllBeds(); // Thay đổi trả về BedDTO
    }
}
