using API.Dtos;
using API.Models;
using AutoMapper;

namespace API.Mapping
{
    public class RuleMapper : Profile
    {
        public RuleMapper()
        {
            CreateMap<Rule, RuleDTO>().ReverseMap();
        }
    }
}
