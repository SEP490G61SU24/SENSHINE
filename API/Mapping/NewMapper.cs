using API.Dtos;
using API.Models;
using AutoMapper;

namespace API.Mapping
{
    public class NewMapper : Profile
    {
        public NewMapper()
        {
            CreateMap<News, NewsDTO>()
                .ForMember(dest => dest.IdNew, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.PublishedDate, opt => opt.MapFrom(src => src.PublishedDate.ToString("yyyy/MM/dd"))).ReverseMap();
            CreateMap<News, NewsDTORequest>()
                .ForMember(dest => dest.PublishedDate, opt => opt.MapFrom(src => src.PublishedDate.ToString("yyyy/MM/dd"))).ReverseMap();
        }
    }
}
