using API.Dtos;
using API.Models;
using AutoMapper;

namespace API.Mapping
{
    public class RuleMapper : Profile
    {
        public RuleMapper()
        {
            CreateMap<Rule, RuleDTO>()
               .ForMember(dest => dest.Ismenu, opt => opt.MapFrom(src => src.Ismenu.HasValue ? src.Ismenu.Value ? "yes" : "no" : "no"))
               .ReverseMap()
               .ForMember(dest => dest.Ismenu, opt => opt.MapFrom(src => src.Ismenu == "yes" ? (bool?)true : src.Ismenu == "no" ? (bool?)false : null));
        }
    }
}
