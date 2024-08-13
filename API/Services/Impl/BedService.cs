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
            // Check if the BedNumber already exists in the same room
            var existingBed = await _context.Beds
                .FirstOrDefaultAsync(b => b.RoomId == bed.RoomId && b.BedNumber == bed.BedNumber);
            if (existingBed != null)
            {
                throw new InvalidOperationException("A bed with the same BedNumber already exists in the same room.");
            }

            await _context.Beds.AddAsync(bed);
            await _context.SaveChangesAsync();
            return bed;
        }

        public async Task<BedDTO> UpdateBedAsync(int id, BedDTO bedDTO)
        {
            var bed = await _context.Beds
                                    .FirstOrDefaultAsync(b => b.Id == id);

            if (bed == null)
            {
                return null;
            }

            // Check if the BedNumber already exists in the same room (but exclude the current bed)
            var existingBed = await _context.Beds
                .FirstOrDefaultAsync(b => b.RoomId == bedDTO.RoomId && b.BedNumber == bedDTO.BedNumber && b.Id != id);
            if (existingBed != null)
            {
                throw new InvalidOperationException("A bed with the same BedNumber already exists in the same room.");
            }

            // Update bed properties
            bed.RoomId = bedDTO.RoomId;
            bed.BedNumber = bedDTO.BedNumber;
            bed.StatusWorking = bedDTO.StatusWorking;

            // Save changes to the database
            _context.Beds.Update(bed);
            await _context.SaveChangesAsync();

            return new BedDTO
            {
                Id = bed.Id,
                RoomId = bed.RoomId,
                BedNumber = bed.BedNumber,
                StatusWorking = bed.StatusWorking
            };
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
