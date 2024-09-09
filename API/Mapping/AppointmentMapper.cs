using API.Models;
using API.Dtos;
using AutoMapper;

public class AppointmentMapper : Profile
{
    public AppointmentMapper()
    {
        CreateMap<Appointment, AppointmentDTO>()
            .ForMember(dest => dest.ServiceIDs, opt => opt.MapFrom(src => src.Services.Select(s => s.Id)))
            .ForMember(dest => dest.ComboIDs, opt => opt.MapFrom(src => src.Combos.Select(c => c.Id).ToList()));
        CreateMap<AppointmentDTO, Appointment>();

        CreateMap<ServiceDTO, Service>();
        CreateMap<Service, ServiceDTO>();

        CreateMap<SlotDTO, Slot>();
        CreateMap<Slot, SlotDTO>();

        CreateMap<UserSlotDTO, UserSlot>();
        CreateMap<UserSlot, UserSlotDTO>();

        CreateMap<BedSlotDTO, BedSlot>();
        CreateMap<BedSlot, BedSlotDTO>();
    }
}