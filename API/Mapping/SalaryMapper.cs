using API.Dtos;
using API.Models;
using AutoMapper;

namespace API.Mapping
{
    public class SalaryMapper : Profile
    {
        public SalaryMapper()
        {
            CreateMap<Salary, SalaryDTO>();
            CreateMap<SalaryDTO, Salary>();
        }
    }
}
