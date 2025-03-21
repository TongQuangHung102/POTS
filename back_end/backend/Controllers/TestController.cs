using backend.DataAccess.DAO;
using backend.Dtos.PracticeAndTest;
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
        private readonly SubjectGradeService _subjectGradeService;

        public TestController(TestService testService, SubjectGradeService subjectGradeService)
        {
            _testService = testService;
            _subjectGradeService = subjectGradeService;
        }

        [HttpGet("get-all-test")]
        public async Task<ActionResult<List<TestDto>>> GetAllTests()
        {
            return await _testService.GetAllTests();
        }

        [HttpGet("get-test-by-grade/{gradeId}/subject/{subjectId}")]
        public async Task<IActionResult> GetTestById(int gradeId, int subjectId)
        {
            var data = await _subjectGradeService.GetTestBySubjectGradeAsync(gradeId, subjectId);
            if (data == null)
                return NotFound("Không tìm thấy bài kiểm tra.");

            var tests = new
            {
                id = data.Id,
                grade = data.Grade.GradeName,
                subject = data.Subject.SubjectName,
                data = data.Tests.Select(x => new TestDto
                {
                    TestId = x.TestId,
                    TestName = x.TestName,
                    CreatedAt = x.CreatedAt,
                    Description = x.Description,
                    DurationInMinutes = x.DurationInMinutes,
                    MaxScore = x.MaxScore,
                    Order = x.Order,
                    IsVisible = x.IsVisible,
                })
            };
            return Ok(tests);
        }

        [HttpPost("add-new-test")]
        public async Task<ActionResult> AddTest([FromBody] TestDto testDto)
        {
            if (testDto == null)
                return BadRequest("Dữ liệu không hợp lệ.");

            await _testService.AddTest(testDto);
            return Ok("Thêm bài kiểm tra thành công!");
        }

        [HttpPut("edit-test/{id}")]
        public async Task<IActionResult> UpdateTest(int id, [FromBody] TestDto testDto)
        {
           return await _testService.UpdateTest(id, testDto);
        }
        [HttpGet("get-test-by-grade/{gradeId}")]
        public async Task<ActionResult<List<TestDto>>> GetTestsBySubjectGradeId(int id)
        {
            var tests = await _testService.GetTestsBySubjectGradeId(id);
            if (tests == null || tests.Count == 0)
                return NotFound("Không tìm thấy bài kiểm tra cho khối lớp này.");

            return Ok(tests);
        }

    }
}
