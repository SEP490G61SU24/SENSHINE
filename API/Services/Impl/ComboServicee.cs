using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Impl
{
    public class ComboServicee : IComboService
    {
        private readonly SenShineSpaContext _dbContext;
        public ComboServicee(SenShineSpaContext dbContext)
        {
            this._dbContext = dbContext;
        }

        public async Task<Combo> CreateComboAsync(Combo combo)
        {
            await _dbContext.Combos.AddAsync(combo);
            await _dbContext.SaveChangesAsync();
            return combo;
        }

        public async Task<Combo> DeleteComboAsync(int Id)
        {
            var existingCombo = await _dbContext.Combos.FirstOrDefaultAsync(x => x.Id == Id);
            if (existingCombo == null)
            {
                return null;
            }
            _dbContext.Combos.Remove(existingCombo);
            await _dbContext.SaveChangesAsync();
            return existingCombo;
        }

        public async Task<Combo> EditComboAsync(int Id, Combo combo)
        {
            var existingCombo = await _dbContext.Combos.FirstOrDefaultAsync(x => x.Id == Id);
            if (existingCombo == null)
            {
                return null;
            }
            existingCombo.Name = combo.Name;
            existingCombo.SalePrice = combo.SalePrice;
            await _dbContext.SaveChangesAsync();

            return existingCombo;
        }

        public async Task<Combo> FindComboWithItsId(int Id)
        {
            return await _dbContext.Combos.FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<List<Combo>> GetAllComboAsync()
        {
            return await _dbContext.Combos.ToListAsync();
        }
    }
}
