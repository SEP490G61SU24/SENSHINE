using API.Models;
using API.Dtos;
using AutoMapper;

public class AppointmentMapper : Profile
{
    public AppointmentMapper()
    {
        CreateMap<AppointmentDTO, Appointment>();
        CreateMap<Appointment, AppointmentDTO>();

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