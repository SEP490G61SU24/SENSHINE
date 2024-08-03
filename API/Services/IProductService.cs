using API.Dtos;
using API.Models;

namespace API.Services
{
    public interface IProductService
    { 
            Task<Product> AddProduct(Product productDto);
            Task<ProductDTO> EditProduct(int id, ProductDTORequest_2 productDto);
            Task<IEnumerable<ProductDTORequest>> ListProduct();
            Task<ProductDTORequest> GetProductDetail(int id);
            Task<ProductDTO> GetProductByName(string name);
            Task<IEnumerable<ProductDTORequest>> GetProductsByFilter(string categoryName, string quantityRange, string priceRange);
            Task<Product> DeleteProductAsync(int id);



    }
}
