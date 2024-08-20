using API.Dtos;
using API.Models;
using API.Ultils;

namespace API.Services
{
    public interface IProductService
    { 
            Task<Product> AddProduct(Product productDto);
            Task<Product> EditProduct(int id, Product productDto);
            Task<IEnumerable<ProductDTORequest>> ListProduct();
            Task<ProductDTORequest> GetProductDetail(int id);
            Task<ProductDTORequest_2> GetProductDetailForEdit(int id);
            Task<ProductDTO> GetProductByName(string name);
            Task<IEnumerable<ProductDTORequest>> GetProductsByFilter(string categoryName, string quantityRange, string priceRange);
            Task<Product?> DeleteProductAsync(int id);
            Task<PaginatedList<ProductDTORequest>> GetProductList(int? spaId = null, int pageIndex = 1, int pageSize = 10, string? searchTerm = null, string? categoryName = null, string? quantityRange = null, string? priceRange=null);



    }
}
