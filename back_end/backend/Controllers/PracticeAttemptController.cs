using backend.Dtos.PracticeAndTest;
using backend.Models;
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

        [HttpGet("history/{lessonId}/{userId}")]
        public async Task<IActionResult> GetHistoryByLessonAndUser(
            int lessonId, int userId, int pageNumber = 1, int pageSize = 10)
        {
            try
            {
                var (history, totalCount) = await _practiceAttemptService.GetHistoryByLessonAndUserAsync(lessonId, userId, pageNumber, pageSize);

                if (history == null || !history.Any())
                {
                    return NotFound(new { Message = "Không tìm thấy lịch sử làm bài cho học sinh này." });
                }

                var response = new
                {
                    TotalCount = totalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)totalCount / pageSize),
                    Data = history
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi, vui lòng thử lại sau!" });
            }
        }

        [HttpGet("history/detail/{practiceId}")]
        public async Task<IActionResult> GetPracticeAttemptDetail(int practiceId)
        {
            try
            {
                var practiceAttempt = await _practiceAttemptService.GetPracticeAttemptDetailAsync(practiceId);
                return Ok(practiceAttempt);
            }
            catch (InvalidOperationException ex)
            {
                return NotFound(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi, vui lòng thử lại sau!" });
            }
        }


    }
}
