using API.Dtos;
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

        public async Task<Combo> DeleteComboAsync(int id)
        {
            // Tìm combo theo ID
            var existingCombo = await _dbContext.Combos
                                                 .Include(c => c.Services) // Bao gồm các dịch vụ liên quan
                                                 .FirstOrDefaultAsync(c => c.Id == id);

            if (existingCombo == null)
            {
                return null;
            }

            // Xóa các liên kết với các dịch vụ nếu cần
            // Ví dụ: nếu có bảng liên kết nhiều-nhiều, có thể cần xử lý nó tại đây
            foreach (var service in existingCombo.Services.ToList())
            {
                // Xóa liên kết giữa combo và dịch vụ
                existingCombo.Services.Remove(service);
            }

            // Xóa combo
            _dbContext.Combos.Remove(existingCombo);
            await _dbContext.SaveChangesAsync();
            return existingCombo;
        }


        public async Task<ComboDTO> EditComboAsync(int id, ComboDTO comboDTO)
        {
            var combo = await _dbContext.Combos
                                        .Include(c => c.Services)
                                        .FirstOrDefaultAsync(x => x.Id == id);

            if (combo == null)
            {
                return null;
            }

            // Cập nhật thông tin combo
            combo.Name = comboDTO.Name;
            combo.Quantity = comboDTO.Quantity;
            combo.Note = comboDTO.Note;
            combo.Discount = comboDTO.Discount;

            // Xử lý cập nhật danh sách dịch vụ
            if (comboDTO.Services != null && comboDTO.Services.Any())
            {
                var serviceIds = comboDTO.Services.Select(s => s.Id).ToList();
                var existingServices = await _dbContext.Services
                                                       .Where(s => serviceIds.Contains(s.Id))
                                                       .ToListAsync();

                if (existingServices.Count != serviceIds.Count)
                {
                    throw new Exception("Một hoặc nhiều dịch vụ không tồn tại.");
                }

                combo.Services = existingServices;
            }
            else
            {
                combo.Services.Clear();
            }

            // Tính lại tổng giá của combo
            combo.Price = combo.Services.Sum(s => s.Amount);

            // Tính lại giá sau giảm giá nếu có
            if (combo.Discount.HasValue && combo.Price.HasValue)
            {
                combo.SalePrice = combo.Price - (combo.Price * combo.Discount / 100);
            }

            await _dbContext.SaveChangesAsync();

            return new ComboDTO
            {
                Id = combo.Id,
                Name = combo.Name,
                Quantity = combo.Quantity,
                Note = combo.Note,
                Price = combo.Price,
                Discount = combo.Discount,
                SalePrice = combo.SalePrice,
                Services = combo.Services.Select(s => new ServiceDTO
                {
                    Id = s.Id,
                    ServiceName = s.ServiceName,
                    Amount = s.Amount,
                    Description = s.Description
                }).ToList()
            };
        }



        public async Task<ComboDTO> FindComboWithItsId(int Id)
        {
            var combo = await _dbContext.Combos
                                        .Include(c => c.Services)
                                        .FirstOrDefaultAsync(x => x.Id == Id);

            if (combo == null)
            {
                return null;
            }

            var comboDTO = new ComboDTO
            {
                Id = combo.Id,
                Name = combo.Name,
                Quantity = combo.Quantity,
                Note = combo.Note,
                Price = combo.Price,
                Discount = combo.Discount,
                SalePrice = combo.SalePrice,
                Services = combo.Services.Select(s => new ServiceDTO
                {
                    Id = s.Id,
                    ServiceName = s.ServiceName,
                    Amount = s.Amount,
                    Description = s.Description
                }).ToList()
            };

            return comboDTO;
        }


        public async Task<List<ComboDTO>> GetAllComboAsync()
        {
            var combos = await _dbContext.Combos
                                         .Include(c => c.Services)
                                         .ToListAsync();

            var comboDTOs = combos.Select(c => new ComboDTO
            {
                Id = c.Id,
                Name = c.Name,
                Quantity = c.Quantity,
                Note = c.Note,
                Price = c.Price,
                Discount = c.Discount,
                SalePrice = c.SalePrice,
                Services = c.Services.Select(s => new ServiceDTO
                {
                    Id = s.Id,
                    ServiceName = s.ServiceName,
                    Amount = s.Amount,
                    Description = s.Description
                }).ToList()
            }).ToList();

            return comboDTOs;
        }
        public async Task<List<Combo>> GetCombosByInvoiceIdAsync(int id)
        {
            // Retrieve the combos associated with the provided invoice ID from the database
            var combos = await _dbContext.Combos.Include(c => c.InvoiceCombos)
                .Where(p => p.InvoiceCombos.Any(c => c.InvoiceId == id)).ToListAsync();
                

            return combos;
        }

    }
}
