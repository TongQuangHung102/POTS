using backend.DataAccess.DAO;
using backend.Dtos.PracticeAndTest;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

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
        [Authorize]
        [HttpGet("get-all-test")]
        public async Task<ActionResult<List<TestDto>>> GetAllTests()
        {
            return await _testService.GetAllTests();
        }
        [Authorize]
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
        [Authorize(Roles = "4")]
        [HttpPost("add-new-test")]
        public async Task<ActionResult> AddTest([FromBody] TestDto testDto)
        {
            try
            {
                await _testService.AddTest(testDto);
                return Ok(new { Message = "Thêm mới thành công!" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = "Bài kiểm tra đã tồn tại", Error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi khi thêm mới.", Error = ex.Message });
            }
        }

        [Authorize(Roles = "4")]
        [HttpPut("edit-test/{id}")]
        public async Task<IActionResult> UpdateTest(int id, [FromBody] TestDto testDto)
        {
            try
            {
                await _testService.UpdateTest(id, testDto);
                return Ok(new { Message = "Cập nhật thành công!" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi khi cập nhật.", Error = ex.Message });
            }
        }
        [Authorize]
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
