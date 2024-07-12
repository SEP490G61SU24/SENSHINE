using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Impl
{
    public class ProvinceService : IProvinceService
    {
        private readonly SenShineSpaContext _context;

        public ProvinceService(SenShineSpaContext context)
        {
            _context = context;
        }
        
        public async Task<Province> GetProvinceByCode(string code)
        {
            return await _context.Provinces.FindAsync(code);
        }

        public async Task<IEnumerable<Province>> GetAllProvinces()
        {
            return await _context.Provinces.ToListAsync();
        }
    }
}
