using API.Dtos;
using API.Models;
using AutoMapper;

namespace API.Mapping
{
    public class WorkScheduleMapper : Profile
    {
        public WorkScheduleMapper()
        {
            CreateMap<WorkSchedule, WorkScheduleDTO>()
                .ForMember(dest => dest.EmployeeName, opt => opt.MapFrom(src => src.Employee.FirstName + " " + src.Employee.MidName + " " + src.Employee.LastName))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Employee.Phone))
                .ReverseMap();
        }
    }
}
