using backend.DataAccess.DAO;
using backend.Dtos;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        private readonly TestService _testService;

        public TestController(TestService testService)
        {
            _testService = testService;
        }

        [HttpGet]
        public async Task<ActionResult<List<TestDto>>> GetAllTests()
        {
            return await _testService.GetAllTests();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TestDto>> GetTestById(int id)
        {
            var test = await _testService.GetTestById(id);
            if (test == null)
                return NotFound("Không tìm thấy bài kiểm tra.");

            return test;
        }

        [HttpPost]
        public async Task<ActionResult> AddTest([FromBody] TestDto testDto)
        {
            if (testDto == null)
                return BadRequest("Dữ liệu không hợp lệ.");

            await _testService.AddTest(testDto);
            return Ok("Thêm bài kiểm tra thành công!");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateTest(int id, [FromBody] TestDto testDto)
        {
            if (testDto == null || id != testDto.TestId)
                return BadRequest("Dữ liệu không hợp lệ.");

            await _testService.UpdateTest(testDto);
            return NoContent();
        }
    }
}
