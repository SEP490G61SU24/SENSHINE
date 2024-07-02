using API.Dtos;
using API.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;


namespace API.Services.Impl
{
    public class ProductService : IProductService
    {
        private readonly SenShineSpaContext _context;
        private readonly IMapper _mapper;

        public ProductService(SenShineSpaContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Product> AddProduct(ProductDTO productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return _mapper.Map<Product>(product);
        }

        public async Task<Product> EditProduct(int id, ProductDTORequest productDto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return null;
            }

            _mapper.Map(productDto, product);
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return _mapper.Map<Product>(product);
        }

        public async Task<IEnumerable<ProductDTO>> ListProduct()
        {
            var products = await _context.Products.ToListAsync();
            return _mapper.Map<IEnumerable<ProductDTO>>(products);
        }

        public async Task<ProductDTORequest> GetProductDetail(int id)
        {
            var product = await _context.Products.FindAsync(id);
            return _mapper.Map<ProductDTORequest>(product);
        }

        public async Task<ProductDTO> GetProductByName(string name)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductName == name);
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<bool> DeleteProduct(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return false;
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return true;
        }

    }

}
