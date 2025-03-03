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
        public async Task<ActionResult<List<TestCategory>>> GetCategories()
        {
            return Ok(await _testCategoryService.GetAllCategories());
        }

        [HttpGet("get-test-category-by-id/{id}")]
        public async Task<ActionResult<TestCategory>> GetCategory(int id)
        {
            var category = await _testCategoryService.GetCategoryById(id);
            if (category == null)
                return NotFound();

            return Ok(category);
        }

        [HttpPost("add-new-test-category")]
        public async Task<IActionResult> AddCategory([FromBody] TestCategory category)
        {
           return await _testCategoryService.AddCategory(category);
        }
        [HttpPut("update-test-category/{id}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] TestCategory category)
        {
           return await _testCategoryService.UpdateCategory(id, category);
        }

    }
}
