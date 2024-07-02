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
    }
}


