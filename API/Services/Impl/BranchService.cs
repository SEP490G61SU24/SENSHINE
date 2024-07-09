using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Impl
{
    public class BranchService : IBranchService
    {
        private readonly SenShineSpaContext _context;

        public BranchService(SenShineSpaContext context)
        {
            _context = context;
        }

        public async Task<Spa> CreateBranch(Spa branch)
        {
            await _context.Spas.AddAsync(branch);
            await _context.SaveChangesAsync();

            return branch;
        }

        public ICollection<Spa> GetBranchs()
        {
            return _context.Spas.ToList();
        }

        public Spa GetBranch(int id)
        {
            var branchs = GetBranchs();
            return branchs.FirstOrDefault(b => b.Id == id);
        }

        public async Task<Spa> UpdateBranch(int id, Spa branch)
        {
            var branchUpdate = await _context.Spas.FirstOrDefaultAsync(s => s.Id == id);
            branchUpdate.SpaName = branch.SpaName;
            branchUpdate.ProvinceCode = branch.ProvinceCode;
            branchUpdate.DistrictCode = branch.DistrictCode;
            branchUpdate.WardCode = branch.WardCode;
            await _context.SaveChangesAsync();

            return branchUpdate;
        }

        public async Task<bool> DeleteBranch(int id)
        {
            var branch = await _context.Spas.FindAsync(id);
            if (branch == null)
            {
                return false;
            }
            _context.Spas.Remove(branch);
            await _context.SaveChangesAsync();
            return true;
        }

        public bool BranchExist(int id)
        {
            return _context.Spas.Any(s => s.Id == id);
        }
    }
}
