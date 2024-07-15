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
                .ForMember(dest => dest.ComboId, opt => opt.MapFrom(src => src.Combos.Select(s => s.Id).ToList()));
                //.ForMember(dest => dest.InvoiceId, opt => opt.MapFrom(src => src.Invoices.Select(s => s.Id).ToList()));
            CreateMap<CardDTO, Card>();
        }
    }
}
