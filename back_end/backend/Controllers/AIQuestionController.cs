using backend.Dtos.AIQuestions;
using backend.Dtos.Curriculum;
using backend.Dtos.Questions;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;


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
            var response = new 
            {
                TotalPage = totalPages,
                Data = questions.Select(q => new 
                {
                    QuestionId = q.QuestionId,
                    QuestionText = q.QuestionText,
                    CreateAt = q.CreateAt,
                    CorrectAnswer = q.CorrectAnswer,
                    CorrectAnswerText = q.AnswerQuestions.FirstOrDefault(a => a.Number == q.CorrectAnswer)?.AnswerText,
                    Status = q.Status,
                    CreateByAI = q.CreateByAI,
                    Level = new LevelSimpleDto
                    {
                        LevelId = q.Level.LevelId,
                        LevelName = q.Level.LevelName
                    },
                    Lesson = new LessonNameDto
                    {
                        LessonName = q.Lesson.LessonName
                    },
                    AnswerQuestions = q.AnswerQuestions.Select(a => new AnswerQuestionDto
                    {
                        AnswerQuestionId = a.AnswerQuestionId,
                        AnswerText = a.AnswerText,
                        Number = a.Number
                    }).ToList()
                }).ToList()
            };

            return Ok(response);
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
        [HttpGet("get-aiquestion-by-id/{questionId}")]
        public async Task<IActionResult> GetAIQuestionById(int questionId)
        {
            var questionDto = await _service.GetAIQuestionByIdAsync(questionId);
            return questionDto != null ? Ok(questionDto) : NotFound("Không tìm thấy câu hỏi.");
        }
        [HttpPut("update-aiquestion/{questionId}")]
        public async Task<IActionResult> UpdateAIQuestion(int questionId, [FromBody] AiQuestionsDto request)
        {
            if (request == null || questionId <= 0 || request.AnswerQuestions == null)
            {
                return BadRequest(new { error = "Dữ liệu không hợp lệ!" });
            }

            bool success = await _service.UpdateAIQuestionAsync(questionId, request);

            return success
                ? Ok(new { message = "Cập nhật câu hỏi AI thành công!" })
                : NotFound(new { error = "Không tìm thấy câu hỏi AI!" });
        }

        [HttpPut("approve-aiquestion/{questionId}")]
        public async Task<IActionResult> ApproveAIQuestion(int questionId)
        {
            var result = await _service.ApproveAIQuestionAsync(questionId);
            return result
                ? Ok(new { message = "Câu hỏi AI đã được phê duyệt." })
                : BadRequest(new { error = "Không thể cập nhật câu hỏi AI." });
        }

    }
}
