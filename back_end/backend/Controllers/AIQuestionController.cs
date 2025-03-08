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

            var success = await _service.GenerateAndSaveAIQuestions(request);
            return success ? Ok("Câu hỏi AI đã được lưu!") : BadRequest("Không thể tạo câu hỏi!");
        }


    }
}
