using API.Dtos;
using API.Models;
using AutoMapper;
using System.Linq;

namespace API.Mapping
{
    public class InvoiceMapper : Profile
    {
        public InvoiceMapper()
        {
            CreateMap<Invoice, InvoiceDTO>()
                .ForMember(dest => dest.CustomerName,
                           opt => opt.MapFrom(src => src.Customer != null ? src.Customer.FirstName + " " + src.Customer.MidName + " " + src.Customer.LastName : null))
                .ForMember(dest => dest.PromotionName,
                           opt => opt.MapFrom(src => src.Promotion != null ? src.Promotion.PromotionName : null))
                .ForMember(dest => dest.DiscountPercentage,
                           opt => opt.MapFrom(src => src.Promotion != null ? src.Promotion.DiscountPercentage : null))
                .ForMember(dest => dest.SpaName,
                           opt => opt.MapFrom(src => src.Spa != null ? src.Spa.SpaName : null))
                .ForMember(dest => dest.ComboIds,
                           opt => opt.MapFrom(src => src.InvoiceCombos.Select(ic => ic.ComboId)))
                .ForMember(dest => dest.ServiceIds,
                           opt => opt.MapFrom(src => src.InvoiceServices.Select(i => i.ServiceId)))
                .ForMember(dest => dest.ComboQuantities,
                           opt => opt.MapFrom(src => src.InvoiceCombos.ToDictionary(ic => ic.ComboId, ic => ic.Quantity)))
                .ForMember(dest => dest.ServiceQuantities,
                           opt => opt.MapFrom(src => src.InvoiceServices.ToDictionary(i => i.ServiceId, i => i.Quantity)))
                .ReverseMap();
            CreateMap<InvoiceCombo, InvoiceComboDTO>()
               .ForMember(dest => dest.Combo, opt => opt.MapFrom(src => src.Combo))
               .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

            CreateMap<InvoiceComboDTO, InvoiceCombo>()
                .ForMember(dest => dest.Combo, opt => opt.Ignore())
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

            CreateMap<InvoiceService, InvoiceServiceDTO>()
               .ForMember(dest => dest.Service, opt => opt.MapFrom(src => src.Service))
               .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));


            // Mapping for InvoiceServiceDTO to InvoiceService
            CreateMap<InvoiceServiceDTO, InvoiceService>()
                .ForMember(dest => dest.Service, opt => opt.Ignore()) // Assuming Service is managed separately
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

           
            CreateMap<InvoiceDTO, Invoice>()
                .ForMember(dest => dest.InvoiceCombos,
                           opt => opt.Ignore())
                .ForMember(dest => dest.InvoiceServices,
                           opt => opt.Ignore())
                .ForMember(dest => dest.Cards,
                           opt => opt.Ignore());

            CreateMap<Card, CardDTO2>()
                .ForMember(dest => dest.CardNumber,
                           opt => opt.MapFrom(src => src.CardNumber))
                .ForMember(dest => dest.CustomerId,
                           opt => opt.MapFrom(src => src.CustomerId))
                .ForMember(dest => dest.CreateDate,
                           opt => opt.MapFrom(src => src.CreateDate))
                .ForMember(dest => dest.Status,
                           opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.BranchId,
                           opt => opt.MapFrom(src => src.BranchId));

            CreateMap<Combo, ComboDTO2>()
                .ForMember(dest => dest.Name,
                           opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Quantity,
                           opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Note,
                           opt => opt.MapFrom(src => src.Note))
                .ForMember(dest => dest.Price,
                           opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.Discount,
                           opt => opt.MapFrom(src => src.Discount))
                .ForMember(dest => dest.SalePrice,
                           opt => opt.MapFrom(src => src.SalePrice)).ReverseMap();

            CreateMap<Service, ServiceDTO2>()
                .ForMember(dest => dest.ServiceName,
                           opt => opt.MapFrom(src => src.ServiceName))
                .ForMember(dest => dest.Amount,
                           opt => opt.MapFrom(src => src.Amount))
                .ForMember(dest => dest.Description,
                           opt => opt.MapFrom(src => src.Description)).ReverseMap();
        }
    }
}
