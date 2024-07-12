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
           .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories));


            CreateMap<Category, CategoryDTO>();
            CreateMap<CategoryDTO, Category>();
        }
        }
}
