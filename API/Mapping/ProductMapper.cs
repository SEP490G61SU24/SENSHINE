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
           .ForMember(dest => dest.CategoryIds, opt => opt.MapFrom(src => src.Categories))
           .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ProductImages.Select(pi => pi.ImageUrl)))
           .ReverseMap();
            CreateMap<Product, ProductDTORequest>()
               .ForMember(dest => dest.Categories, opt => opt.MapFrom(src => src.Categories.Select(c => new CategoryDTO
               {
                   Id=c.Id,
                   CategoryName = c.CategoryName
               })))
               .ForMember(dest => dest.ProductImages, opt => opt.MapFrom(src => src.ProductImages.Select(pi => new ProductImageDTO
               { 
                    ProductId = src.Id,
                    ImageUrl = pi.ImageUrl
                   }))).ReverseMap();
            CreateMap<ProductDTORequest_2, Product>()
            .ForMember(dest => dest.Categories, opt => opt.Ignore())
            .ForMember(dest => dest.ProductImages, opt => opt.Ignore()).ReverseMap();
            

            CreateMap<Category, CategoryDTO>().ReverseMap();
            CreateMap<ProductImage, ProductImageDTO>().ReverseMap();


            
    





        }
    }
}
