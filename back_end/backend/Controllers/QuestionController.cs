using backend.Dtos;
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

        public QuestionController(QuestionService questionService)
        {
            _questionService = questionService;
        }

        [HttpGet("get-all-question")]
        public async Task<IActionResult> GetAllQuestions(
            [FromQuery] int? lessonId,
            [FromQuery] int? levelId,
            [FromQuery] bool? isVisible,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return await _questionService.GetAllQuestionsAsync(lessonId, levelId, isVisible, page, pageSize);
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
    }
}
