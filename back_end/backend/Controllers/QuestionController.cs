using backend.Dtos.Dashboard;
using backend.Dtos.Questions;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class QuestionController : ControllerBase
    {
        private readonly QuestionService _questionService;
        private readonly LessonService _lessonService;

        public QuestionController(QuestionService questionService, LessonService lessonService)
        {
            _questionService = questionService;
            _lessonService = lessonService;
        }

        [HttpGet("get-all-question")]
        public async Task<QuestionResponseDto> GetAllQuestions(
            [FromQuery] int? chapterId,
            [FromQuery] int? lessonId,
            [FromQuery] int? levelId,
            [FromQuery] bool? isVisible,
            [FromQuery] string? searchTerm,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
            )
        {
            return await _questionService.GetAllQuestionsAsync(chapterId, lessonId, levelId, searchTerm, isVisible, page, pageSize);
        }

        [HttpGet("get-question-by/{questionId}")]
        public async Task<IActionResult> GetQuestionById(int questionId)
        {
            return await _questionService.GetQuestionByIdAsync(questionId);
        }
        [HttpPut("edit-question/{questionId}")]
        public async Task<IActionResult> UpdateQuestion(int questionId, [FromBody] QuestionDto questionDto)
        {
            return await _questionService.UpdateQuestionAsync(questionId, questionDto);
        }
        [HttpPost("add-question")]
        public async Task<IActionResult> AddQuestion([FromBody] CreateQuestionDto questionDto)
        {
            return await _questionService.AddQuestionAsync(questionDto);
        }

        [HttpPost("gen-question-practice")]
        public async Task<IActionResult> GetGeneratedQuestion([FromBody] QuestionRequest request)
        {
            if (request == null)
            {
                return BadRequest("Dữ liệu request không hợp lệ.");
            }

            var (questions, byAi) = await _questionService.GenQuestionAIForPractice(request);

            if (questions == null || questions.Count == 0)
            {
                return NotFound("Không tìm thấy câu hỏi.");
            }
            var response = new
            {
                byAi,
                questions = questions.Select(question => new
                {
                    question.QuestionText,
                    question.CorrectAnswer,
                    AnswerQuestions = question.AnswerQuestions.Select(a => new
                    {
                        a.AnswerText,
                        a.Number
                    }).ToList()
                }).ToList()
            };

            return Ok(response);
        }

        [HttpPost("generate-test")]
        public async Task<IActionResult> GenerateTestQuestions([FromBody] GenerateTestRequest request)
        {
            try
            {
                var questions = await _questionService.GenerateTestQuestionsAsync(request);
                var response = questions.Select(question => new 
                {
                    question.QuestionId,
                    question.QuestionText,
                    question.CreateAt,
                    question.CorrectAnswer,
                    question.IsVisible,
                    question.CreateByAI,
                    Level = new
                    {
                        question.Level.LevelName
                    },
                    Lesson = new
                    {
                        question.Lesson.LessonName
                    },
                    AnswerQuestions = question.AnswerQuestions.Select(a => new
                    {
                        a.AnswerQuestionId,
                        a.AnswerText,
                        a.Number
                    }).ToList()
                });
                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

    }
}
