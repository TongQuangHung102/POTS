using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestCategoryController : ControllerBase
    {
        private readonly TestCategoryService _testCategoryService;

        public TestCategoryController(TestCategoryService testCategoryService)
        {
            _testCategoryService = testCategoryService;
        }

        [HttpGet("get-all-test-category")]
        public async Task<IActionResult> GetAllCategories()
        {
            try
            {
                var categories = await _testCategoryService.GetAllCategoriesAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpGet("get-test-category-by-id/{id}")]
        public async Task<IActionResult> GetCategoryById(int id)
        {
            try
            {
                var category = await _testCategoryService.GetCategoryByIdAsync(id);
                if (category == null) return NotFound(new { Message = "Danh mục không tồn tại." });
                return Ok(category);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

        [HttpPost("add-new-test-category")]
        public async Task<IActionResult> AddCategory([FromBody] TestCategory category)
        {
            try
            {
                var result = await _testCategoryService.AddCategoryAsync(category);
                return result ? Ok(new { Message = "Thêm danh mục thành công." }) : BadRequest(new { Message = "Không thể thêm danh mục." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }
        [HttpPut("update-test-category/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] TestCategory category)
        {
            try
            {
                var result = await _testCategoryService.UpdateCategoryAsync(id, category);
                return result ? Ok(new { Message = "Cập nhật danh mục thành công." }) : NotFound(new { Message = "Không tìm thấy danh mục." });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = ex.Message });
            }
        }

    }
}
