using API.Dtos;
using API.Models;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers
{
    [Route("api")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly IMapper _mapper;
        private readonly SenShineSpaContext _dbContext;

        public ProductController(SenShineSpaContext dbContext, IProductService productService, IMapper mapper)
        {
            this._dbContext = dbContext;
            _productService = productService;
            _mapper = mapper;
        }

        [HttpPost("AddProduct")]
        public async Task<ActionResult<Product>> AddProduct([FromBody] ProductDTORequest_2 productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Map DTO to Product entity
                var newProduct = _mapper.Map<Product>(productDto);

                // Ensure the categories being added to the product are existing ones
                if (productDto.CategoryIds != null && productDto.CategoryIds.Any())
                {
                    var existingCategories = await _dbContext.Categories
                                                             .Where(c => productDto.CategoryIds.Contains(c.Id))
                                                             .ToListAsync();

                    if (existingCategories.Count != productDto.CategoryIds.Count)
                    {
                        return BadRequest("Một hoặc nhiều danh mục không tồn tại.");
                    }

                    // Add categories to the product
                    foreach (var category in existingCategories)
                    {
                        newProduct.Categories.Add(category);
                    }
                }

                // Save the product to get the ProductId
                _dbContext.Products.Add(newProduct);
                await _dbContext.SaveChangesAsync();

                // Add images after the product is saved
                if (productDto.ImageUrls != null)
                {
                    foreach (var imageUrl in productDto.ImageUrls)
                    {
                        var productImage = new ProductImage
                        {
                            ProductId = newProduct.Id, 
                            ImageUrl = imageUrl
                        };
                        _dbContext.ProductImages.Add(productImage);
                    }
                }

                // Save images to the database
                await _dbContext.SaveChangesAsync();

                // Map the created product back to DTO for response
                var createdProductDto = _mapper.Map<ProductDTORequest_2>(newProduct);

                return CreatedAtAction(nameof(GetProductDetail), new { id = newProduct.Id }, createdProductDto);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }



        [HttpPut("EditProduct/{id}")]
        public async Task<IActionResult> EditProduct(int id, [FromBody] ProductDTORequest_2 productDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                // Fetch the existing product with related entities
                var existingProduct = await _dbContext.Products
                    .Include(p => p.Categories)
                    .Include(p => p.ProductImages)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (existingProduct == null)
                {
                    return NotFound("Product not found.");
                }

                // Map ProductDTORequest_2 to the existing entity
                existingProduct.ProductName = productDto.ProductName;
                existingProduct.Price = productDto.Price;
                existingProduct.Quantity = (int)productDto.Quantity;

                // Handle Category updates
                if (productDto.CategoryIds != null && productDto.CategoryIds.Any())
                {
                    var categoryIds = productDto.CategoryIds.ToList();
                    var existingCategories = await _dbContext.Categories
                        .Where(c => categoryIds.Contains(c.Id))
                        .ToListAsync();

                    if (existingCategories.Count != productDto.CategoryIds.Count)
                    {
                        return BadRequest("One or more categories do not exist.");
                    }

                    // Clear existing categories and add the new ones
                    existingProduct.Categories.Clear();
                    foreach (var category in existingCategories)
                    {
                        existingProduct.Categories.Add(category);
                    }
                }
                else
                {
                    existingProduct.Categories.Clear();
                }

                // Handle Image updates
                if (productDto.ImageUrls != null)
                {
                    // Remove existing images
                    _dbContext.ProductImages.RemoveRange(existingProduct.ProductImages);

                    // Add new images
                    foreach (var imageUrl in productDto.ImageUrls)
                    {
                        var productImage = new ProductImage
                        {
                            ProductId = existingProduct.Id,
                            ImageUrl = imageUrl
                        };
                        _dbContext.ProductImages.Add(productImage);
                    }
                }
                else
                {
                    _dbContext.ProductImages.RemoveRange(existingProduct.ProductImages);
                }

                // Save changes to the database
                await _dbContext.SaveChangesAsync();

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }

        [HttpGet("ListAllProduct")]
        public async Task<ActionResult<IEnumerable<ProductDTORequest>>> ListProduct()
        {
            try
            {
                var products = await _productService.ListProduct();
                return Ok(products);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }

        [HttpGet("GetProductDetailById/{id}")]
        public async Task<ActionResult<ProductDTORequest>> GetProductDetail(int id)
        {
            try
            {
                var product = await _productService.GetProductDetail(id);
                if (product == null)
                {
                    return NotFound();
                }

                return Ok(product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }

        [HttpGet("GetProductDetailForEdit/{id}")]
        public async Task<ActionResult<ProductDTORequest_2>> GetProductDetailForEdit(int id)
        {
            var product = await _productService.GetProductDetailForEdit(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpGet("GetFilterProducts")]
        public async Task<IActionResult> GetFilteredProducts(string? categoryName = null, string? quantityRange = null, string? priceRange = null)
        {
            try
            {
                var products = await _productService.GetProductsByFilter(categoryName, quantityRange, priceRange);
                return Ok(products);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }

        [HttpGet("GetProductDetailByName/{name}")]
        public async Task<ActionResult<ProductDTO>> GetProductByName(string name)
        {
            try
            {
                var product = await _productService.GetProductByName(name);
                if (product == null)
                {
                    return NotFound();
                }

                return Ok(product);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }

        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            try
            {
                var product = await _productService.DeleteProductAsync(id);
                if (product == null)
                {
                    return NotFound();
                }

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }

        [HttpGet("GetProductsPaging")]
        public async Task<IActionResult> GetAllProductsPaging([FromQuery] int? spaId=null,[FromQuery] int pageIndex = 1, [FromQuery] int pageSize = 10, [FromQuery] string? searchTerm = null,[FromQuery]string? categoryName = null,[FromQuery] string? quantityRange = null,[FromQuery] string? priceRange = null)
        {
            try
            {
                if (pageIndex < 1 || pageSize < 1)
                {
                    return BadRequest("Chỉ số trang hoặc kích thước trang không hợp lệ.");
                }

                var pageData = await _productService.GetProductList(spaId,pageIndex, pageSize, searchTerm, categoryName,quantityRange,priceRange);
                return Ok(pageData);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Có lỗi xảy ra: " + ex.Message);
            }
        }
    }

}
