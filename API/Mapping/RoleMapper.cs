using API.Dtos;
using API.Models;
using AutoMapper;

namespace API.Mapping
{
    public class RoleMapper : Profile
    {
        public RoleMapper()
        {
            CreateMap<Role, RoleDTO>().ReverseMap();
        }        
    }
}
