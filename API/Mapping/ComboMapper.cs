using API.Dtos;
using API.Models;
using AutoMapper;

namespace API.Mapping
{
    public class ComboMapper : Profile
    {

        public ComboMapper()
        {
            // Mapping từ ComboDTO sang Combo
            CreateMap<ComboDTO, Combo>()
                .ForMember(dest => dest.Services, opt => opt.Ignore()).ReverseMap();
            CreateMap<ComboDTO2, Combo>()
                .ReverseMap();
            // Mapping từ ServiceDTO sang Service
            CreateMap<ServiceDTO, Service>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()).ReverseMap(); // Ignore Id mapping to prevent setting explicit values
        }
    }
}
