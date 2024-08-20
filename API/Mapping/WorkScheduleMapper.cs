using API.Dtos;
using API.Models;
using AutoMapper;

namespace API.Mapping
{
    public class WorkScheduleMapper : Profile
    {
        public WorkScheduleMapper()
        {
            CreateMap<WorkScheduleDTO, WorkSchedule>()
                .ForMember(dest => dest.EmployeeId, opt => opt.Ignore())
                .ForMember(dest => dest.StartDateTime, opt => opt.MapFrom(src => src.StartDateTime))
                .ForMember(dest => dest.EndDateTime, opt => opt.MapFrom(src => src.EndDateTime))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status));

            CreateMap<WorkSchedule, WorkScheduleDTO>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FirstName + " " + src.Employee.MidName + " " + src.Employee.LastName))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Employee.Phone))
                .ReverseMap();
        }
    }
}
