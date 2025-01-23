using eCommerceApp.Application.DTOs.Product;
using eCommerceApp.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceApp.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController(IProductService productService) : ControllerBase
    {
        private readonly IProductService _productServ = productService;

        [HttpGet("All")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _productServ.GetAllAsync();

            return data.Any() ? Ok(data) : NotFound(data);   
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var data = await _productServ.GetByIdAsync(id);

            return data != null ? Ok(data) : NotFound(data);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(CreateProduct product)
        {
           var result = await _productServ.AddAsync(product);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(UpdateProduct product)
        {
            var result = await _productServ.UpdateAsync(product);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _productServ.DeleteAsync(id);

            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
