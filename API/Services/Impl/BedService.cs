using API.Dtos;
using API.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services.Impl
{
    public class BedService : IBedService
    {
        private readonly SenShineSpaContext _context;

        public BedService(SenShineSpaContext context)
        {
            _context = context;
        }

       
        public async Task<Bed> AddBed(Bed bed)
        {
            await _context.Beds.AddAsync(bed);
            await _context.SaveChangesAsync();
            return bed;
        }

        public async Task<Bed> UpdateBed(int id, string bedNumber)
        {
            var bed = await _context.Beds.FindAsync(id);
            if (bed == null)
            {
                return null;
            }

            bed.BedNumber = bedNumber;

            _context.Beds.Update(bed);
            await _context.SaveChangesAsync();
            return bed;
        }

        public async Task<bool> DeleteBed(int id)
        {
            var bed = await _context.Beds.FindAsync(id);
            if (bed == null)
            {
                return false;
            }

            _context.Beds.Remove(bed);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<BedDTO> GetBedById(int id)
        {
            var bed = await _context.Beds
                                    .Include(b => b.Room)
                                    .FirstOrDefaultAsync(b => b.Id == id);

            if (bed == null)
            {
                return null;
            }

            return new BedDTO
            {
                Id = bed.Id,
                RoomId = bed.RoomId,
                BedNumber = bed.BedNumber,
                StatusWorking = bed.StatusWorking,
                
            };
        }

        public async Task<IEnumerable<BedDTO>> GetAllBeds()
        {
            return await _context.Beds
                                 .Include(b => b.Room)
                                 .Select(b => new BedDTO
                                 {
                                     Id = b.Id,
                                     RoomId = b.RoomId,
                                     BedNumber = b.BedNumber,
                                     StatusWorking = b.StatusWorking,
                                     
                                 })
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Bed>> GetBedByRoomId(int roomId)
        {
            return await _context.Beds.Include(b => b.Room)  
                                 .Where(b => b.RoomId == roomId)  
                                 .ToListAsync();
        }
    }
}
