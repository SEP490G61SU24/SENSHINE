using API.Dtos;
using API.Models;
using API.Services;
using AutoMapper;
using Microsoft.AspNetCore.Http;
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
                // Convert ProductDTORequest_2 to Product entity using AutoMapper
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

                // Ensure the images being added to the product are existing ones
               /* if (productDto.ImageIds != null && productDto.ImageIds.Any())
                {
                    var existingImages = await _dbContext.Images
                                                         .Where(i => productDto.ImageIds.Contains(i.Id))
                                                         .ToListAsync();

                    if (existingImages.Count != productDto.ImageIds.Count)
                    {
                        return BadRequest("Một hoặc nhiều hình ảnh không tồn tại.");
                    }

                    // Add images to the product
                    foreach (var image in existingImages)
                    {
                        newProduct.ProductImages.Add(new ProductImage { ProductId = newProduct.Id, ImageUrl = image.ImageUrl });
                    }
                }*/

                // Add the product using the service
                var createdProduct = await _productService.AddProduct(newProduct);

                var createdProductDto = _mapper.Map<ProductDTORequest_2>(createdProduct);

                return CreatedAtAction(nameof(GetProductDetail), new { id = createdProduct.Id }, createdProductDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Lỗi khi tạo sản phẩm mới: {ex.Message}");
            }
        }



        [HttpPut("EditProduct/{id}")]
        public async Task<IActionResult> EditProduct(int id, ProductDTORequest_2 productDto)
        {
            var product = await _productService.EditProduct(id, productDto);
            if (product == null)
            {
                return NotFound();
            }

            return NoContent();
        }

        [HttpGet("ListAllProduct")]
        public async Task<ActionResult<IEnumerable<ProductDTORequest>>> ListProduct()
        {
            var products = await _productService.ListProduct();
            return Ok(products);
        }

        [HttpGet("GetProductDetailById/{id}")]
        public async Task<ActionResult<ProductDTORequest>> GetProductDetail(int id)
        {
            var product = await _productService.GetProductDetail(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }
        [HttpGet("GetFilterProducts")]
        public async Task<IActionResult> GetFilteredProducts(
    string? categoryName = null,
    string? quantityRange = null,
    string? priceRange = null)
        {
            var products = await _productService.GetProductsByFilter(categoryName, quantityRange, priceRange);
            return Ok(products);
        }

        [HttpGet("GetProductDetailByName/{name}")]
        public async Task<ActionResult<ProductDTO>> GetProductByName(string name)
        {
            var product = await _productService.GetProductByName(name);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
        }

        [HttpDelete("DeleteProduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var product = await _productService.DeleteProductAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return NoContent();
        }
    }

}
