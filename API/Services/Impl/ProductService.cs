using API.Dtos;
using API.Models;
using API.Ultils;
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

        public async Task<Product> EditProduct(int id, Product productDto)
        {
            // Find the existing product
            var product = await _context.Products
                .Include(p => p.Categories) 
                .FirstOrDefaultAsync(p => p.Id == id);

            
           

            _mapper.Map(productDto, product);

            
            _context.Products.Update(product);

            
            await _context.SaveChangesAsync();

            
            return _mapper.Map<Product>(product);
        }

        public async Task<IEnumerable<ProductDTORequest>> ListProduct()
        {
            var products = await _context.Products.Include(p=>p.Categories).Include(p=>p.ProductImages).ToListAsync();
            return _mapper.Map<IEnumerable<ProductDTORequest>>(products);
        }

        public async Task<ProductDTORequest> GetProductDetail(int id)
        {
            
            var product = await _context.Products
                .Include(p => p.Categories).Include(p=>p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            
            if (product == null)
            {
                return null; 
            }

            
            var productDTORequest = _mapper.Map<ProductDTORequest>(product);

            return productDTORequest;
        }
        public async Task<ProductDTORequest_2> GetProductDetailForEdit(int id)
        {
            var product = await _context.Products
                .Include(p => p.Categories)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
            {
                return null;
            }

            var productDTORequest_2 = _mapper.Map<ProductDTORequest_2>(product);

            return productDTORequest_2;
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
                .Include(p => p.Categories).Include(P => P.ProductImages)
                .ToListAsync();


            if (!string.IsNullOrEmpty(categoryName))
            {
                products = products
                    .Where(p => p.Categories.Any(pc => pc.CategoryName.Equals(categoryName, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
            }

            return _mapper.Map<IEnumerable<ProductDTORequest>>(products);
        }






        public async Task<Product?> DeleteProductAsync(int id)
        {
            // Retrieve the product including related entities
            var existingProduct = await _context.Products
                .Include(p => p.Categories)   
                .Include(p => p.ProductImages) 
                .FirstOrDefaultAsync(p => p.Id == id);

            // Return null if the product doesn't exist
            if (existingProduct == null)
            {
                return null;
            }

           
            existingProduct.Categories.Clear();

            // Remove related images
            if (existingProduct.ProductImages != null && existingProduct.ProductImages.Any())
            {
                _context.ProductImages.RemoveRange(existingProduct.ProductImages);
            }

            // Remove the product itself
            _context.Products.Remove(existingProduct);

            // Save changes to the database
            await _context.SaveChangesAsync();

            return existingProduct;
        }

        public async Task<FilteredPaginatedList<ProductDTORequest>> GetProductList(int? spaId = null, int pageIndex = 1, int pageSize = 10, string searchTerm = null, string? categoryName = null, string? quantityRange = null, string? priceRange = null)
        {
            var quantityParts = quantityRange?.Split('-');
            var priceParts = priceRange?.Split('-');

            int? minQuantity = quantityParts != null && int.TryParse(quantityParts[0], out var qMin) ? (int?)qMin : null;
            int? maxQuantity = quantityParts != null && int.TryParse(quantityParts[1], out var qMax) ? (int?)qMax : null;

            decimal? minPrice = priceParts != null && decimal.TryParse(priceParts[0], out var pMin) ? (decimal?)pMin : null;
            decimal? maxPrice = priceParts != null && decimal.TryParse(priceParts[1], out var pMax) ? (decimal?)pMax : null;

            IQueryable<Product> query = _context.Products.Include(p => p.Categories).Include(P => P.ProductImages).AsQueryable();

            if (spaId.HasValue)
            {
                query = query.Where(x => x.SpaId == spaId);
            }
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
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(u => u.ProductName.Contains(searchTerm));
            }
            if (!string.IsNullOrEmpty(categoryName))
            {
                query = query.Where(u => u.Categories.Any(x => x.CategoryName.Equals(categoryName)));
            }
            // Đếm tổng số bản ghi để tính tổng số trang
            var count = await query.CountAsync();

            // Lấy danh sách với phân trang
            var news = await query.Skip((pageIndex - 1) * pageSize)
                                   .Take(pageSize)
                                   .ToListAsync();
            var newsDtos = _mapper.Map<IEnumerable<ProductDTORequest>>(news);

            return new FilteredPaginatedList<ProductDTORequest>
            {
                Items = newsDtos,
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = count,
            };
        }
    }

}


