using API.Models;
using API.Dtos;
using AutoMapper;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        CreateMap<AppointmentDTO, Appointment>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status == "true"))
            .ForMember(dest => dest.Services, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.Employee, opt => opt.Ignore())
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src => src.Products));

        CreateMap<Appointment, AppointmentDTO>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status ? "true" : "false"))
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

        CreateMap<AppointmentDTO.AppointmentProductDTO, Product>()
            .ForMember(dest => dest.Id, opt => opt.Ignore()) // Giả sử Product đã tồn tại trong cơ sở dữ liệu, bạn có thể không cần ánh xạ Id
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductName));

    }
}