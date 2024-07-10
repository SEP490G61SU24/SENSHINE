using API.Dtos;
using API.Models;

namespace API.Services
{
    public interface IProductService
    { 
            Task<Product> AddProduct(ProductDTO productDto);
            Task<Product> EditProduct(int id, ProductDTORequest productDto);
            Task<IEnumerable<ProductDTO>> ListProduct();
            Task<ProductDTORequest> GetProductDetail(int id);
            Task<ProductDTO> GetProductByName(string name);
            Task<bool> DeleteProduct(int id);
        

    }
}
