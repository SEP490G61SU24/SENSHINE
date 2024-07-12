using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Impl
{
    public class WardService : IWardService
    {
        private readonly SenShineSpaContext _context;

        public WardService(SenShineSpaContext context)
        {
            _context = context;
        }

        public async Task<Ward> GetWardByCode(string code)
        {
            return await _context.Wards.FindAsync(code);
        }

        public async Task<IEnumerable<Ward>> GetAllWards()
        {
            return await _context.Wards.ToListAsync();
        }
    }
}
