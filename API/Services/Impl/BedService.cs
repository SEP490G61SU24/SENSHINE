using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Impl
{
    public class BedService : IBedService
    {
        private readonly SenShineSpaContext _context;

        public BedService(SenShineSpaContext context)
        {
            _context = context;
        }

        public async Task<Bed> AddBed(string bedNumber)
        {
            var bed = new Bed
            {
                BedNumber = bedNumber
            };

            _context.Beds.Add(bed);
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

        public async Task<Bed> GetBedById(int id)
        {
            return await _context.Beds.FindAsync(id);
        }

        public async Task<IEnumerable<Bed>> GetAllBeds()
        {
            return await _context.Beds.ToListAsync();
        }
    }
}
