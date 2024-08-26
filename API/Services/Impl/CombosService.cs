using API.Dtos;
using API.Models;
using API.Ultils;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Impl
{
    public class CombosService : IComboService
    {
        private readonly SenShineSpaContext _dbContext;
        private readonly IMapper _mapper;

        public CombosService(SenShineSpaContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public async Task<Combo> CreateComboAsync(Combo combo)
        {
            if (combo == null)
                throw new ArgumentException("Combo cannot be null.");

            try
            {
                await _dbContext.Combos.AddAsync(combo);
                await _dbContext.SaveChangesAsync();
                return combo;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while creating the combo.", ex);
            }
        }

        public async Task<Combo> DeleteComboAsync(int id)
        {
            var existingCombo = await _dbContext.Combos.Include(c => c.Services)
                                                       .FirstOrDefaultAsync(c => c.Id == id);

            if (existingCombo == null)
                throw new ArgumentException("Combo not found.");

            try
            {
                existingCombo.Services.Clear();  // Clear the services
                _dbContext.Combos.Remove(existingCombo);
                await _dbContext.SaveChangesAsync();
                return existingCombo;
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while deleting the combo.", ex);
            }
        }

        public async Task<ComboDTO> EditComboAsync(int id, ComboDTO comboDTO)
        {
            var combo = await _dbContext.Combos.Include(c => c.Services).FirstOrDefaultAsync(x => x.Id == id);

            if (combo == null)
                throw new ArgumentException("Combo not found.");

            try
            {
                combo.Name = comboDTO.Name;
                combo.Quantity = comboDTO.Quantity;
                combo.Note = comboDTO.Note;
                combo.Discount = comboDTO.Discount;

                if (comboDTO.Services != null && comboDTO.Services.Any())
                {
                    var serviceIds = comboDTO.Services.Select(s => s.Id).ToList();
                    var existingServices = await _dbContext.Services.Where(s => serviceIds.Contains(s.Id)).ToListAsync();

                    if (existingServices.Count != serviceIds.Count)
                        throw new ArgumentException("One or more services do not exist.");

                    combo.Services = existingServices;
                }
                else
                {
                    combo.Services.Clear();
                }

                combo.Price = combo.Services.Sum(s => s.Amount);

                if (combo.Discount.HasValue && combo.Price.HasValue)
                {
                    combo.SalePrice = combo.Price - (combo.Price * combo.Discount / 100);
                }

                await _dbContext.SaveChangesAsync();

                return _mapper.Map<ComboDTO>(combo);
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while updating the combo.", ex);
            }
        }

        public async Task<ComboDTO> FindComboWithItsId(int id)
        {
            var combo = await _dbContext.Combos.Include(c => c.Services).FirstOrDefaultAsync(x => x.Id == id);

            if (combo == null)
                throw new ArgumentException("Combo not found.");

            return _mapper.Map<ComboDTO>(combo);
        }

        public async Task<List<ComboDTO>> GetAllComboAsync()
        {
            var combos = await _dbContext.Combos.Include(c => c.Services).ToListAsync();
            return combos.Select(c => _mapper.Map<ComboDTO>(c)).ToList();
        }

        public async Task<PaginatedList<ComboDTO2>> GetComboList(int pageIndex = 1, int pageSize = 10, string searchTerm = null)
        {
            IQueryable<Combo> query = _dbContext.Combos.Include(c => c.Services);

            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u => u.Name.Contains(searchTerm));
            }

            var count = await query.CountAsync();
            var combos = await query.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            var comboDtos = _mapper.Map<IEnumerable<ComboDTO2>>(combos);

            return new PaginatedList<ComboDTO2>
            {
                Items = comboDtos,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = count,
            };
        }

        public async Task<List<Combo>> GetCombosByInvoiceIdAsync(int id)
        {
            var combos = await _dbContext.Combos.Include(c => c.InvoiceCombos)
                                                .Where(p => p.InvoiceCombos.Any(c => c.InvoiceId == id))
                                                .ToListAsync();

            if (combos == null || !combos.Any())
                throw new ArgumentException("No combos found for the specified invoice ID.");

            return combos;
        }
    }
}
