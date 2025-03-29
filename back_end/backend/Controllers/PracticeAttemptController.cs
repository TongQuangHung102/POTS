using backend.Dtos.PracticeAndTest;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PracticeAttemptController : ControllerBase
    {
        private readonly PracticeAttemptService _practiceAttemptService;

        public PracticeAttemptController(PracticeAttemptService practiceAttemptService)
        {
            _practiceAttemptService = practiceAttemptService;
        }

        [HttpPost("add-practice-attempt")]
        public async Task<IActionResult> AddPracticeAttempt([FromBody] PracticeAttemptDto dto)
        {
            try
            {
                await _practiceAttemptService.AddPraticeAttempt(dto);
                return Ok(new { message = "Thành công!" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi.", error = ex.Message });
            }
        }
    }
}
