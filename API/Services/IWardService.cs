using API.Models;

namespace API.Services
{
    public interface IWardService
    {
        Task<Ward> GetWardByCode(string code);
        Task<IEnumerable<Ward>> GetAllWards();
    }
}
