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

        public async Task<IEnumerable<PromotionDTORequest>> ListPromotion()
        {
            var promotions = await _context.Promotions.ToListAsync();
            return _mapper.Map<IEnumerable<PromotionDTORequest>>(promotions);
        }

        public async Task<PromotionDTO> GetPromotionDetail(int id)
        {
            var promotion = await _context.Promotions.FindAsync(id);
            if (promotion == null)
            {
                return null;
            }

            return _mapper.Map<PromotionDTO>(promotion);
        }

        public async Task<IEnumerable<PromotionDTORequest>> PromotionByCode(string code)
        {
            var promotions = await _context.Promotions
                                            .Where(x => x.PromotionName == code)
                                            .ToListAsync();

            return _mapper.Map<IEnumerable<PromotionDTORequest>>(promotions);
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
