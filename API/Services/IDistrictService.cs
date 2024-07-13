using API.Models;

namespace API.Services
{
    public interface IDistrictService
    {
        Task<District> GetDistrictByCode(string code);
        Task<IEnumerable<District>> GetAllDistricts();
        Task<IEnumerable<District>> GetDistrictsByProvinceCode(string provinceCode);

    }
}
