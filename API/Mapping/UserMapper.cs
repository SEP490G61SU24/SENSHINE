using AutoMapper;
using API.Dtos;
using API.Models;
using API.Services;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<User, UserDTO>();
        CreateMap<UserDTO, User>();

        CreateMap<User, UserDTO>()
           .ForMember(dest => dest.Roles, opt => opt.MapFrom(src => src.Roles))
           .ForMember(dest => dest.Address, opt => opt.MapFrom<CustomAddressResolver>());
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
