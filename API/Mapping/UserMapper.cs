using AutoMapper;
using API.Dtos;
using API.Models;
using API.Services;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<User, UserDto>();
        CreateMap<UserDto, User>();

        CreateMap<User, UserDto>()
           .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles))
           .ForMember(dest => dest.Address, opt => opt.MapFrom<CustomAddressResolver>());

        CreateMap<UserDto, User>();
        CreateMap<Role, RoleDTO>();
        CreateMap<RoleDTO, Role>();
    }
}

public class CustomAddressResolver : IValueResolver<User, UserDto, string>
{
    private readonly IUserService _userService;

    public CustomAddressResolver(IUserService userService)
    {
        _userService = userService;
    }

    public string Resolve(User source, UserDto destination, string destMember, ResolutionContext context)
    {
        var address = _userService.GetAddress(source.WardCode, source.DistrictCode, source.ProvinceCode).Result;
        return address;
    }
}
