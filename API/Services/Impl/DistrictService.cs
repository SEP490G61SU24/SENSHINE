using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Impl
{
    public class DistrictService : IDistrictService
    {
        private readonly SenShineSpaContext _context;

        public DistrictService(SenShineSpaContext context)
        {
            _context = context;
        }

        public async Task<District> GetDistrictByCode(string code)
        {
            return await _context.Districts.FindAsync(code);
        }

        public async Task<IEnumerable<District>> GetAllDistricts()
        {
            return await _context.Districts.ToListAsync();
        }
    }
}
