using API.Dtos;
using API.Models;
using AutoMapper;

namespace API.Mapping
{
    public class BedMapper : Profile
    {
        public BedMapper()
        {
            CreateMap<Bed, BedDTO>()
                .ReverseMap()
                .ForMember(dest => dest.Room, opt => opt.Ignore());
        }
    }
}
