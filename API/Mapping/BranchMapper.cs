using API.Dtos;
using API.Models;
using AutoMapper;

namespace API.Mapping
{
    public class BranchMapper : Profile
    {
        public BranchMapper() 
        {
            CreateMap<Spa, BranchDTO>();
            CreateMap<BranchDTO, Spa>();
            CreateMap<Bed, BedDTO>();
            CreateMap<BedDTO, Bed>();
            CreateMap<Room, RoomDTO>();
            CreateMap<RoomDTO, Room>();

            CreateMap<Bed, BedDTO>();
            CreateMap<BedDTO, Bed>();
        }
    }
}
