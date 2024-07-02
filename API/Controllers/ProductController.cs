using API.Dtos;
using API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    [Route("api")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        [HttpPost("AddProduct")]
        public async Task<ActionResult<ProductDTO>> AddProduct(ProductDTO productDto)
        {
            var product = await _productService.AddProduct(productDto);
            return CreatedAtAction(nameof(GetProductDetail), new { id = product.Id }, product);
        }

        [HttpPut("EditProduct/{id}")]
        public async Task<IActionResult> EditProduct(int id, ProductDTORequest productDto)
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
        public async Task<ActionResult<ProductDTO>> GetProductDetail(int id)
        {
            var product = await _productService.GetProductDetail(id);
            if (product == null)
            {
                return NotFound();
            }

            return Ok(product);
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

        [HttpDelete("Deleteproduct/{id}")]
        public async Task<IActionResult> DeleteProduct(int id)
        {
            var result = await _productService.DeleteProduct(id);
            if (!result)
            {
                return NotFound();
            }

            return NoContent();
        }
    }

}
