using API.Models;
using API.Dtos;
using AutoMapper;

public class AppointmentMapper : Profile
{
    public AppointmentMapper()
    {
        CreateMap<AppointmentDTO, Appointment>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Services, opt => opt.Ignore())
            .ForMember(dest => dest.Customer, opt => opt.Ignore())
            .ForMember(dest => dest.Employee, opt => opt.Ignore());

        CreateMap<Appointment, AppointmentDTO>()
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status))
            .ForMember(dest => dest.Services, opt => opt.MapFrom(src => src.Services.Select(s => new ServiceDTO
            {
                Id = s.Id,
                ServiceName = s.ServiceName,
                Amount = s.Amount,
                Description = s.Description
            }).ToList()));

        CreateMap<ServiceDTO, Service>();
        CreateMap<Service, ServiceDTO>();
    }
}