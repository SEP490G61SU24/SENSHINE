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
            try
            {
                await _context.Spas.AddAsync(branch);
                await _context.SaveChangesAsync();

                return branch;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("Error creating branch.", ex);
            }
        }

        public ICollection<Spa> GetBranchs()
        {
            try
            {
                return _context.Spas.ToList();
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception("Error retrieving branches.", ex);
            }
        }

        public Spa GetBranch(int id)
        {
            try
            {
                var branchs = GetBranchs();
                return branchs.FirstOrDefault(b => b.Id == id);
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception($"Error retrieving branch with ID {id}.", ex);
            }
        }

        public async Task<Spa> UpdateBranch(int id, Spa branch)
        {
            try
            {
                var branchUpdate = await _context.Spas.FirstOrDefaultAsync(s => s.Id == id);
                if (branchUpdate != null)
                {
                    branchUpdate.SpaName = branch.SpaName;
                    branchUpdate.ProvinceCode = branch.ProvinceCode;
                    branchUpdate.DistrictCode = branch.DistrictCode;
                    branchUpdate.WardCode = branch.WardCode;
                    await _context.SaveChangesAsync();
                }

                return branchUpdate;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception($"Error updating branch with ID {id}.", ex);
            }
        }

        public async Task<bool> DeleteBranch(int id)
        {
            try
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
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception($"Error deleting branch with ID {id}.", ex);
            }
        }

        public bool BranchExist(int id)
        {
            try
            {
                return _context.Spas.Any(s => s.Id == id);
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception($"Error checking existence of branch with ID {id}.", ex);
            }
        }

        public int? GetBranchIdByUserId(int id)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(s => s.Id == id);
                return user?.SpaId;
            }
            catch (Exception ex)
            {
                // Handle or log the exception as needed
                throw new Exception($"Error retrieving branch ID for user with ID {id}.", ex);
            }
        }
    }
}
