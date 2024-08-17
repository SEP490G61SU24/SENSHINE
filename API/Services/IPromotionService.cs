using API.Dtos;
using API.Models;

namespace API.Services
{
    public interface IPromotionService
    {
        Task<Promotion> AddPromotion(PromotionDTORequest PromotionDto);
        Task<Promotion> EditPromotion(int id, PromotionDTORequest PromotionDto);
        Task<IEnumerable<PromotionDTORespond>> ListPromotion();
        Task<PromotionDTORespond> GetPromotionDetail(int id);
       
        Task<IEnumerable<PromotionDTORespond>> GetPromotionsByFilter(string spaLocation, DateTime? startDate, DateTime? endDate);
        Task<bool> DeletePromotion(int id);
    }
}
