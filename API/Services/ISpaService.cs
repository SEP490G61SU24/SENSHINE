using API.Dtos;
using API.Models;
using API.Ultils;

namespace API.Services
{
    public interface ISpaService
    {
        //tim kiem tat ca cac service
        Task<List<Service>> GetAllServiceAsync();

        //tim kiem service theo id
        Task<Service> FindServiceWithItsId(int Id);
        //them 1 service moi
        Task<Service> CreateServiceAsync(Service services);
        //edit 1 service 
        Task<Service> EditServiceAsync(int Id, Service services);
        //xoa 1 service
        Task<Service> DeleteServiceAsync(int Id);
        Task<bool> ValidateServicesAsync(AppointmentDTO appointmentDTO);
        Task<PaginatedList<ServiceDTO>> GetServices(int pageIndex, int pageSize, string searchTerm);
    }
}
