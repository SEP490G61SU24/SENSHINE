using API.Dtos;
using API.Models;
using API.Ultils;

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
        Task<FilteredPaginatedList<PromotionDTORespond>> GetPromotionListBySpaId(int? spaId=null, int pageIndex = 1, int pageSize = 10, string searchTerm = null,DateTime? startDate= null, DateTime? endDate = null);
    }
}
