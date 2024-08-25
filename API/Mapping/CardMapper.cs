using API.Dtos;
using API.Models;
using AutoMapper;

namespace API.Mapping
{
    public class CardMapper : Profile
    {
        public CardMapper()
        {
            CreateMap<Card, CardDTO>()
                //.ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.FirstName + " " + src.Customer.MidName + " " + src.Customer.LastName))
                //.ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Customer.Phone))
                .ForMember(dest => dest.CardComboId, opt => opt.MapFrom(src => src.CardCombos.Select(c => c.Id).ToList()));
            CreateMap<CardDTO, Card>();

            CreateMap<CardCombo, CardComboDTO>()
                .ForMember(dest => dest.ComboId, opt => opt.MapFrom(src => src.Combo.Id));
            CreateMap<CardComboDTO, CardCombo>();

            CreateMap<CardInvoice, CardInvoiceDTO>()
                .ForMember(dest => dest.InvoiceId, opt => opt.MapFrom(src => src.Invoice.Id));
            CreateMap<CardInvoiceDTO, CardInvoice>();
        }
    }
}
