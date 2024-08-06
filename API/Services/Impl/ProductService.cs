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

        public async Task<Product> AddProduct(Product productDto)
        {
            var product = _mapper.Map<Product>(productDto);
            _context.Products.Add(product);
            await _context.SaveChangesAsync();

            return _mapper.Map<Product>(product);
        }

        public async Task<ProductDTO> EditProduct(int id, ProductDTORequest_2 productDto)
        {
            var product = await _context.Products
                .Include(p => p.Categories)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return null;
            }

            // Clear existing categories
            product.Categories.Clear();

            // Add new categories
            foreach (var categoryId in productDto.CategoryIds)
            {
                var category = await _context.Categories.FindAsync(categoryId);
                if (category != null)
                {
                    product.Categories.Add(category);
                }
            }

            // Update product properties
            _mapper.Map(productDto, product);
            _context.Products.Update(product);
            await _context.SaveChangesAsync();

            return _mapper.Map<ProductDTO>(product);
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
        public async Task<IEnumerable<ProductDTORequest>> GetProductsByFilter(string categoryName, string quantityRange, string priceRange)
        {
            var quantityParts = quantityRange?.Split('-');
            var priceParts = priceRange?.Split('-');

            int? minQuantity = quantityParts != null && int.TryParse(quantityParts[0], out var qMin) ? (int?)qMin : null;
            int? maxQuantity = quantityParts != null && int.TryParse(quantityParts[1], out var qMax) ? (int?)qMax : null;

            decimal? minPrice = priceParts != null && decimal.TryParse(priceParts[0], out var pMin) ? (decimal?)pMin : null;
            decimal? maxPrice = priceParts != null && decimal.TryParse(priceParts[1], out var pMax) ? (decimal?)pMax : null;

            var query = _context.Products.AsQueryable();

            
            if (minQuantity.HasValue)
            {
                query = query.Where(p => p.Quantity >= minQuantity.Value);
            }

            if (maxQuantity.HasValue)
            {
                query = query.Where(p => p.Quantity <= maxQuantity.Value);
            }

            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            
            var products = await query
                .Include(p => p.Categories)
                .ToListAsync();

            
            if (!string.IsNullOrEmpty(categoryName))
            {
                products = products
                    .Where(p => p.Categories.Any(pc => pc.CategoryName.Equals(categoryName, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            return _mapper.Map<IEnumerable<ProductDTORequest>>(products);
        }





        public async Task<Product> DeleteProductAsync(int id)
        {
            
            var existingProduct = await _context.Products
                                                  //.Include(p => p.Images) 
                                                  .Include(p => p.Categories) 
                                                  .FirstOrDefaultAsync(p => p.Id == id);

            if (existingProduct == null)
            {
                
                return null;
            }

            // Handle related entities if needed
            // Example: If there is a many-to-many relationship, you might need to remove links here
            /*foreach (var image in existingProduct.Images.ToList())
            {
                // Optionally: Remove image from product if needed
                existingProduct.Images.Remove(image);
            }*/

            foreach (var category in existingProduct.Categories.ToList())
            {
                
                existingProduct.Categories.Remove(category);
            }

           
            _context.Products.Remove(existingProduct);
            await _context.SaveChangesAsync();

            return existingProduct;
        }


    }

}
