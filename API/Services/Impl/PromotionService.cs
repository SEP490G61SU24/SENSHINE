using API.Dtos;
using API.Models;
using API.Ultils;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace API.Services.Impl
{
    public class PromotionService : IPromotionService
    {
        private readonly SenShineSpaContext _context;
        private readonly IMapper _mapper;

        public PromotionService(SenShineSpaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Promotion> AddPromotion(PromotionDTORequest promotionDto)
        {
            var promotion = _mapper.Map<Promotion>(promotionDto);
            _context.Promotions.Add(promotion);
            await _context.SaveChangesAsync();

            return promotion;
        }

        public async Task<Promotion> EditPromotion(int id, PromotionDTORequest promotionDto)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null)
            {
                return null;
            }

            _mapper.Map(promotionDto, promotion);
            _context.Promotions.Update(promotion);
            await _context.SaveChangesAsync();

            return promotion;
        }

        public async Task<IEnumerable<PromotionDTORespond>> ListPromotion()
        {
            var promotions = await _context.Promotions.Include(x => x.Spa).ToListAsync();
            return _mapper.Map<IEnumerable<PromotionDTORespond>>(promotions);
        }

        public async Task<PromotionDTORespond> GetPromotionDetail(int id)
        {
            var promotion = await _context.Promotions.Include(x => x.Spa).FirstOrDefaultAsync(p => p.Id == id);
            if (promotion == null)
            {
                return null;
            }

            return _mapper.Map<PromotionDTORespond>(promotion);
        }

        public async Task<IEnumerable<PromotionDTORespond>> GetPromotionsByFilter(string spaLocation, DateTime? startDate = null, DateTime? endDate = null)
        {
            var query = _context.Promotions.Include(x => x.Spa).AsQueryable();

            if (!string.IsNullOrEmpty(spaLocation) && spaLocation != "All Location")
            {
                query = query.Where(x => x.Spa.SpaName == spaLocation);
            }

            if (startDate.HasValue)
            {
                query = query.Where(x => x.StartDate >= startDate);
            }

            if (endDate.HasValue)
            {
                query = query.Where(x => x.EndDate <= endDate);
            }

            if (!string.IsNullOrEmpty(spaLocation) || startDate.HasValue || endDate.HasValue)
            {
                var promotions = await query.ToListAsync();
                return _mapper.Map<IEnumerable<PromotionDTORespond>>(promotions);
            }
            else
            {
                var allPromotions = await _context.Promotions.Include(x => x.Spa).ToListAsync();
                return _mapper.Map<IEnumerable<PromotionDTORespond>>(allPromotions);
            }
        }

        public async Task<bool> DeletePromotion(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null)
            {
                return false;
            }

            _context.Promotions.Remove(promotion);
            await _context.SaveChangesAsync();

            return true;
        }

        public async Task<PaginatedList<PromotionDTORespond>> GetPromotionListBySpaId(int? spaId =null,int pageIndex = 1, int pageSize = 10, string searchTerm = null, DateTime? startDate = null,DateTime? endDate = null)
        {
            // Tạo query cơ bản
            IQueryable<Promotion> query = _context.Promotions.Include(x=>x.Spa).AsQueryable();

            if (startDate.HasValue)
            {
                query = query.Where(x => x.StartDate >= startDate);
            }

            if (endDate.HasValue)
            {
                query = query.Where(x => x.EndDate <= endDate);
            }
            if (spaId.HasValue)
            {
                query = query.Where(x => x.SpaId == spaId);
            }
            // Nếu có searchTerm, thêm điều kiện tìm kiếm vào query
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u => u.Description.Contains(searchTerm) ||
                                         u.PromotionName.Contains(searchTerm) );
            }

            // Đếm tổng số bản ghi để tính tổng số trang
            var count = await query.CountAsync();

            // Lấy danh sách với phân trang
            var news = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();
            var newsDtos = _mapper.Map<IEnumerable<PromotionDTORespond>>(news);

            return new PaginatedList<PromotionDTORespond>
            {
                Items = newsDtos,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = count,
            };
        }




    }
}


