using API.Models;

namespace API.Services
{
    public interface IProvinceService
    {
        Task<Province> GetProvinceByCode(string code);
        Task<IEnumerable<Province>> GetAllProvinces();
    }
}
