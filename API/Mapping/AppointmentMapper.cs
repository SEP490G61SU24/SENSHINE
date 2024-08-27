using API.Models;
using API.Dtos;
using AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<AppointmentDTO, Appointment>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Services, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.Employee, opt => opt.Ignore());

        CreateMap<Appointment, AppointmentDTO>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Customer, opt => opt.MapFrom(src => new AppointmentUserDTO
            {
                Id = src.Customer.Id,
                FullName = $"{src.Customer.FirstName} {src.Customer.MidName} {src.Customer.LastName}",
                Phone = src.Customer.Phone,
                Address = $"{src.Customer.ProvinceCode} {src.Customer.DistrictCode} {src.Customer.WardCode}"
            }))
            .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => new AppointmentUserDTO
            {
                Id = src.Employee.Id,
                FullName = $"{src.Employee.FirstName} {src.Employee.MidName} {src.Employee.LastName}",
                Phone = src.Employee.Phone
            }))
            .ForMember(dest => dest.Services, opt => opt.MapFrom(src => src.Services.Select(s => new ServiceDTO
            {
                Id = s.Id,
                ServiceName = s.ServiceName,
                Amount = s.Amount,
                Description = s.Description
            }).ToList()));
    }
}