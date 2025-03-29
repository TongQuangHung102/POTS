using backend.Dtos.PracticeQuestion;
using backend.Dtos.Questions;
using backend.Repositories;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PracticeQuestionController : Controller
    {
        private readonly PracticeQuestionService _practiceQuestionService;

        public PracticeQuestionController(PracticeQuestionService practiceQuestionService)
        {
            _practiceQuestionService = practiceQuestionService;
        }

        [HttpPost("save-questions")]
        public async Task<IActionResult> SaveQuestions([FromBody] List<PracticeQuestionDto> questionList, [FromQuery] int lessonId)
        {
            if (questionList == null || questionList.Count == 0)
            {
                return BadRequest("Question list cannot be null or empty.");
            }

            try
            {
                await _practiceQuestionService.SaveQuestionAndAnswersAsync(questionList, lessonId);

                return Ok("Questions and answers saved successfully.");
            }
            catch (InvalidOperationException ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest($"Invalid input: {ex.Message}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An unexpected error occurred: {ex.Message}");
            }
        }
    }
}
