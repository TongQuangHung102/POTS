using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;


namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AIQuestionController : ControllerBase
    {
        private readonly AIQuestionService _service;

        public AIQuestionController(AIQuestionService service)
        {
            _service = service;
        }

        [HttpPost("generate-ai-questions")]
        public async Task<IActionResult> GenerateAIQuestions([FromBody] AIQuestionRequestDto request)
        {
            Console.WriteLine("=== DỮ LIỆU NHẬN ĐƯỢC ===");
            Console.WriteLine("Question: " + request.Question);
            Console.WriteLine("NumQuestions: " + request.NumQuestions);

            if (request == null)
            {
                return BadRequest("Dữ liệu không hợp lệ!");
            }

            if (string.IsNullOrWhiteSpace(request.Question))
            {
                return BadRequest("Thiếu trường 'Question'");
            }

            var generatedQuestionIds = await _service.GenerateAndSaveAIQuestions(request);
            if (generatedQuestionIds.Count == 0)
            {
                return BadRequest(new { error = "Không thể tạo câu hỏi!" });
            }

            return Ok(new
            {
                message = "Câu hỏi AI đã được lưu!",
                generatedQuestionIds
            });
        }


        [HttpGet("get-all-aiquestion")]
        public async Task<IActionResult> GetAllAIQuestions(
            [FromQuery] int lessonId,
            [FromQuery] int? levelId,
            [FromQuery] string? status,
            [FromQuery] DateTime? createdAt,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            if (lessonId <= 0)
            {
                return BadRequest("LessonId không hợp lệ!");
            }

            var (questions, totalPages) = await _service.GetAIQuestionsByFilters(lessonId, levelId, status, createdAt, pageNumber, pageSize);

            return questions.Count > 0
                ? Ok(new { questions, totalPages })
                : NotFound("Không tìm thấy câu hỏi nào.");
        }

        [HttpPut("update-lesson-id")]
        public async Task<IActionResult> UpdateLessonId([FromBody] UpdateLessonIdRequestDto request)
        {
            if (request == null || request.LessonId <= 0 || request.AiQuestionIds == null || !request.AiQuestionIds.Any())
            {
                return BadRequest(new { error = "Dữ liệu không hợp lệ!" });
            }

            var success = await _service.UpdateLessonIdAsync(request.LessonId, request.AiQuestionIds);

            return success
                ? Ok(new { message = "Cập nhật LessonId thành công!" })
                : BadRequest(new { error = "Cập nhật LessonId thất bại!" });
        }

    }
}
