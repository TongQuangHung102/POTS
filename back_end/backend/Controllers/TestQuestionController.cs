using backend.Dtos.Questions;
using backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestQuestionController : ControllerBase
    {
        private readonly TestQuestionService _testQuestionService;

        public TestQuestionController(TestQuestionService testQuestionService)
        {
            _testQuestionService = testQuestionService;
        }

        [HttpPost("add-questions")]
        public async Task<IActionResult> AddQuestionsToTest([FromBody] AddQuestionsToTestDto dto)
        {
            try
            {
                await _testQuestionService.AddQuestionsToTestAsync(dto);
                return Ok(new { Message = "Thêm mới thành công!" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi khi thêm câu hỏi vào bài kiểm tra.", Error = ex.Message });
            }
        }


        [HttpGet("get-test-questions")]
        public async Task<IActionResult> GetTestQuestions(int testId)
        {
            try
            {
                var questions = await _testQuestionService.GetTestQuestionsAsync(testId);

                var response = new
                {
                    Data = questions.Select(q => new
                    {
                        q.QuestionId,
                        q.Question.QuestionText,
                        q.Question.CreateAt,
                        q.Question.CorrectAnswer,
                        CorrectAnswerText = q.Question.AnswerQuestions.FirstOrDefault(a => a.Number == q.Question.CorrectAnswer)?.AnswerText,
                        q.Question.IsVisible,
                        q.Question.CreateByAI,
                        Level = new
                        {
                            q.Question.Level.LevelName,
                            q.Question.Level.LevelId
                        },
                        Lesson = new
                        {
                            q.Question.Lesson.LessonName
                        },
                        AnswerQuestions = q.Question.AnswerQuestions.Select(a => new
                        {
                            a.AnswerQuestionId,
                            a.AnswerText,
                            a.Number
                        }).ToList()
                    }).ToList(),

                    Test = new
                    {
                        TestName = questions.FirstOrDefault()?.Test?.TestName,
                        DurationInMinutes = questions.FirstOrDefault()?.Test?.DurationInMinutes,
                        IsVisible = questions.FirstOrDefault()?.Test?.IsVisible
                    }
                };

                return Ok(response);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi khi truy vấn dữ liệu.", Error = ex.Message });
            }
        }

        [HttpPut("update-questions")]
        public async Task<IActionResult> UpdateTestQuestions([FromBody] AddQuestionsToTestDto dto)
        {
            try
            {
                await _testQuestionService.UpdateTestQuestionsAsync(dto);
                return Ok(new { Message = "Cập nhật thành công!" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi khi cập nhật câu hỏi của bài kiểm tra.", Error = ex.Message });
            }
        }


    }
}
