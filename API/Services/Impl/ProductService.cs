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

        public async Task<IEnumerable<ProductDTORequest>> ListProduct()
        {
            var products = await _context.Products.Include(p=>p.Categories).ToListAsync();
            return _mapper.Map<IEnumerable<ProductDTORequest>>(products);
        }

        public async Task<ProductDTORequest> GetProductDetail(int id)
        {
            
            var product = await _context.Products
                .Include(p => p.Categories) 
                .FirstOrDefaultAsync(p => p.Id == id);

            
            if (product == null)
            {
                return null; 
            }

            
            var productDTORequest = _mapper.Map<ProductDTORequest>(product);

            return productDTORequest;
        }


        public async Task<ProductDTO> GetProductByName(string name)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductName == name);
            return _mapper.Map<ProductDTO>(product);
        }

        public async Task<bool> DeleteProduct(int id)
        {
            
            var product = await _context.Products
               /* .Include(p => p.ProductCategories)*/
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return false;
            }

            
           /* _context.ProductCategories.RemoveRange(product.ProductCategories);*/

            
            _context.Products.Remove(product);

            try
            {
               
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateException)
            {
                
                return false;
            }

            return true;
        }


    }

}
