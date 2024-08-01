using API.Dtos;
using API.Models;
using AutoMapper;

namespace API.Mapping
{
    public class ProductMapper:Profile
    {
        public ProductMapper()
        {
            CreateMap<Product, ProductDTO>()
           .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories)).ReverseMap();
            CreateMap<Product, ProductDTORequest>()
               .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories.Select(c => new CategoryDTO
               {
                   
                   CategoryName = c.CategoryName
               })))
               .ReverseMap();


            CreateMap<Category, CategoryDTO>().ReverseMap();
            
        }
        }
}
