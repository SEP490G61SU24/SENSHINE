using API.Dtos;
using API.Models;
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

        public async Task<IEnumerable<PromotionDTORespond>> GetPromotionsByFilter(string spaLocation, DateTime? startDate, DateTime? endDate)
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

        




    }
}


