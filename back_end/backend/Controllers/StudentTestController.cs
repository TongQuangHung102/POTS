using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StudentTestController : ControllerBase
    {
        private readonly StudentTestService _studentTestService;

        public StudentTestController(StudentTestService studentTestService)
        {
            _studentTestService = studentTestService;
        }

        [HttpPost("add-student-test")]
        public async Task<IActionResult> AddStudentTest([FromBody] StudentTestDto studentTestDto)
        {
            try
            {
                await _studentTestService.AddStudentTestAsync(studentTestDto);
                return Ok(new { message = "Thêm thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi.", error = ex.Message });
            }
        }
    }
}
