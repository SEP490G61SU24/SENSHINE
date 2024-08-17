using AutoMapper;
using API.Dtos;
using API.Models;
using API.Services;

public class UserMapper : Profile
{
    public UserMapper()
    {
        _ = CreateMap<User, UserDTO>()
           .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles))
           .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.Roles.FirstOrDefault().Id))
           .ForMember(dest => dest.RoleName, opt => opt.MapFrom(src => src.Roles.FirstOrDefault().RoleName))
           .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FirstName + " " + src.MidName + " " + src.LastName))
           .ForMember(dest => dest.Address, opt => opt.MapFrom<CustomAddressResolver>())
           .ReverseMap();
    }
}

public class CustomAddressResolver : IValueResolver<User, UserDTO, string>
{
    private readonly IUserService _userService;

    public CustomAddressResolver(IUserService userService)
    {
        _userService = userService;
    }

    public string Resolve(User source, UserDTO destination, string destMember, ResolutionContext context)
    {
        var address = _userService.GetAddress(source.WardCode, source.DistrictCode, source.ProvinceCode).Result;
        return address;
    }
}
