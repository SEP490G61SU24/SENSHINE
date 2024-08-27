using API.Dtos;
using API.Models;
using Microsoft.EntityFrameworkCore;

namespace API.Services.Impl
{
    public class SpaService : ISpaService
    {
        private readonly SenShineSpaContext _dbContext;
        public SpaService(SenShineSpaContext dbContext)
        {
            this._dbContext = dbContext;
        }

        //Tạo service mới
        public async Task<Service> CreateServiceAsync(Service services)
        {
            await _dbContext.Services.AddAsync(services);
            await _dbContext.SaveChangesAsync();
            return services;
        }

        //Xóa service
        public async Task<Service> DeleteServiceAsync(int Id)
        {
            var existingService = await _dbContext.Services.FirstOrDefaultAsync(x => x.Id == Id);
            if (existingService == null)
            {
                return null;
            }
            _dbContext.Services.Remove(existingService);
            await _dbContext.SaveChangesAsync();
            return existingService;
        }

        //Sửa thông tin service
        public async Task<Service> EditServiceAsync(int Id, Service services)
        {
            var existingService = await _dbContext.Services.FirstOrDefaultAsync(x => x.Id == Id);
            if (existingService == null)
            {
                return null;
            }
            existingService.ServiceName = services.ServiceName;
            existingService.Description = services.Description;
            await _dbContext.SaveChangesAsync();

            return existingService;
        }

        public async Task<Service> FindServiceWithItsId(int Id)
        {
            return await _dbContext.Services.FirstOrDefaultAsync(x => x.Id == Id);
        }

        public async Task<List<Service>> GetAllServiceAsync()
        {
            return await _dbContext.Services.ToListAsync();
        }

        public async Task<List<Service>> GetServicesByInvoiceIdAsync(int id)
        {
            var services = await _dbContext.Services.Include(c => c.InvoiceServices)
                .Where(p => p.InvoiceServices.Any(c => c.InvoiceId == id)).ToListAsync();
            return services;
        }

        public async Task<bool> ValidateServicesAsync(AppointmentDTO appointmentDTO)
        {
            var serviceIds = appointmentDTO.Services.Select(s => s.Id).ToList();

            var existingServices = await _dbContext.Services
                                                   .Where(s => serviceIds.Contains(s.Id))
                                                   .ToListAsync();

            // Return true if the count matches, indicating all services exist
            return existingServices.Count == serviceIds.Count;
        }
    }
}


