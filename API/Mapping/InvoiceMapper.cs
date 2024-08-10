using API.Dtos;
using API.Models;
using AutoMapper;

namespace API.Mapping
{
    public class InvoiceMapper : Profile

    {
        public InvoiceMapper()
        {
            CreateMap<Invoice, InvoiceDTO>()
            .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer != null ? src.Customer.FirstName +src.Customer.MidName+src.Customer.LastName : null))
            .ForMember(dest => dest.PromotionName, opt => opt.MapFrom(src => src.Promotion != null ? src.Promotion.PromotionName : null))
            .ForMember(dest => dest.SpaName, opt => opt.MapFrom(src => src.Spa != null ? src.Spa.SpaName : null))
            .ForMember(dest => dest.Cards, opt => opt.MapFrom(src => src.Cards))
            .ForMember(dest => dest.Combos, opt => opt.MapFrom(src => src.Combos))
            .ForMember(dest => dest.Services, opt => opt.MapFrom(src => src.Services))
            .ForMember(dest => dest.CardIds, opt => opt.MapFrom(src => src.Cards.Select(c => c.Id)))
            .ForMember(dest => dest.ComboIds, opt => opt.MapFrom(src => src.Combos.Select(c => c.Id)))
            .ForMember(dest => dest.ServiceIds, opt => opt.MapFrom(src => src.Services.Select(s => s.Id)));


            CreateMap<InvoiceDTO, Invoice>()
            .ForMember(dest => dest.Cards, opt => opt.Ignore())
            .ForMember(dest => dest.Combos, opt => opt.Ignore())
            .ForMember(dest => dest.Services, opt => opt.Ignore());

            CreateMap<Card, CardDTO2>()
                .ForMember(dest => dest.CardNumber, opt => opt.MapFrom(src => src.CardNumber))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.CreateDate, opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.BranchId, opt => opt.MapFrom(src => src.BranchId));

            CreateMap<Combo, ComboDTO2>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Note, opt => opt.MapFrom(src => src.Note))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => src.Discount))
                .ForMember(dest => dest.SalePrice, opt => opt.MapFrom(src => src.SalePrice));

            CreateMap<Service, ServiceDTO2>()
                .ForMember(dest => dest.ServiceName, opt => opt.MapFrom(src => src.ServiceName))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));

        }
    }
}
