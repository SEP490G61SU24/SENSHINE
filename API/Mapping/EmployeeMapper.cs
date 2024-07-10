using API.Dtos;
using API.Models;
using AutoMapper;

namespace API.Mapping
{
    public class EmployeeMapper : Profile
    {
        public EmployeeMapper()
        {
            CreateMap<User, EmployeeDTO>()
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Roles.Select(s => s.Id).ToList()));
            CreateMap<EmployeeDTO, User>();
        }
    }
}
