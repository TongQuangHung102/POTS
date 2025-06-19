using backend.Dtos.Dashboard;
using backend.Dtos.Questions;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

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
        [Authorize(Roles = "4")]
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
        [Authorize(Roles = "4")]
        [HttpGet("get-question-by/{questionId}")]
        public async Task<IActionResult> GetQuestionById(int questionId)
        {
            var question = await _questionService.GetQuestionByIdAsync(questionId);
            var response = new
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
            };

            return Ok(response);
        }
        [Authorize(Roles = "4")]
        [HttpPut("edit-question/{questionId}")]
        public async Task<IActionResult> UpdateQuestion(int questionId, [FromBody] QuestionDto questionDto)
        {
            try
            {
                await _questionService.UpdateQuestionAsync(questionId, questionDto);
                return Ok(new { message = "Cập nhật câu hỏi và danh sách câu trả lời thành công." });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi cập nhật câu hỏi.", error = ex.Message });
            }
        }
        [Authorize(Roles = "4")]
        [HttpPost("add-question")]
        public async Task<IActionResult> AddQuestion([FromBody] CreateQuestionDto questionDto)
        {
            try
            {
                await _questionService.AddQuestionAsync(questionDto);
                return Ok(new { message = "Câu hỏi và câu trả lời đã được thêm thành công." });
            }
            catch (ArgumentNullException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi khi thêm câu hỏi.", error = ex.Message });
            }
        }
        [Authorize(Roles = "1")]
        [HttpPost("gen-question-practice")]
        public async Task<IActionResult> GetGeneratedQuestion([FromBody] QuestionRequest request)
        {
            try
            {
                var isTrialString = User.FindFirst("IsTrial")?.Value;
                bool? isTrial = null;
                if (!string.IsNullOrEmpty(isTrialString))
                {
                    isTrial = bool.TryParse(isTrialString, out var trial) ? trial : (bool?)null;
                }

                var expirationDateString = User.FindFirst("ExpirationDate")?.Value;
                DateTime? expirationDate = null;
                if (!string.IsNullOrEmpty(expirationDateString))
                {
                    expirationDate = DateTime.TryParse(expirationDateString, out var expDate) ? expDate : (DateTime?)null;
                }

                var maxExercisesString = User.FindFirst("maxExercises")?.Value;
                int? maxExercises = null;
                if (!string.IsNullOrEmpty(maxExercisesString))
                {
                    maxExercises = int.TryParse(maxExercisesString, out var exercises) ? exercises : (int?)null;
                }

                var (questions, byAi, firstTime) = await _questionService.GenQuestionAIForPractice(request, isTrial, expirationDate, maxExercises);

                if (questions == null || questions.Count == 0)
                {
                    return NotFound();
                }

                var response = new
                {
                    byAi,
                    firstTime,
                    questions = questions.Select(question => new
                    {
                        question.QuestionText,
                        question.CorrectAnswer,
                        question.QuestionId,
                        AnswerQuestions = question.AnswerQuestions.Select(a => new
                        {
                            a.AnswerText,
                            a.Number
                        }).ToList()
                    }).ToList()
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [Authorize(Roles = "4")]
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
