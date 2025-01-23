using eCommerceApp.Application.DTOs.Category;
using eCommerceApp.Application.DTOs.Product;
using eCommerceApp.Application.Services.Implementations;
using eCommerceApp.Application.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eCommerceApp.Host.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController(ICategoryService categoryService) : ControllerBase
    {
        private readonly ICategoryService _categoryServ = categoryService;

        [HttpGet("All")]
        public async Task<IActionResult> GetAll()
        {
            var data = await _categoryServ.GetAllAsync();

            return data.Any() ? Ok(data) : NotFound(data);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetProductById(Guid id)
        {
            var data = await _categoryServ.GetByIdAsync(id);

            return data != null ? Ok(data) : NotFound(data);
        }

        [HttpPost("add")]
        public async Task<IActionResult> Add(CreateCategory category)
        {
            var result = await _categoryServ.AddAsync(category);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpPut("update")]
        public async Task<IActionResult> Update(UpdateCategory category)
        {
            var result = await _categoryServ.UpdateAsync(category);

            return result.Success ? Ok(result) : BadRequest(result);
        }

        [HttpDelete("delete")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _categoryServ.DeleteAsync(id);

            return result.Success ? Ok(result) : BadRequest(result);
        }
    }
}
