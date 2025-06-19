using backend.Dtos.PracticeAndTest;
using backend.Dtos.Questions;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "1")]
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

                var data = practiceAttempt.StudentAnswers.Select(sa => new
                {
                    AnswerId = sa.AnswerId,
                    PracticeQuestionId = sa.PracticeQuestion?.PracticeQuestionId,
                    QuestionId = sa.Question?.QuestionId,
                    SelectedAnswer = sa.SelectedAnswer,
                    Question = new
                    {
                        QuestionId = sa.PracticeQuestion?.PracticeQuestionId ?? sa.Question?.QuestionId,
                        QuestionText = sa.PracticeQuestion?.QuestionText ?? sa.Question?.QuestionText,
                        CorrectAnswer = sa.PracticeQuestion?.CorrectAnswer ?? sa.Question?.CorrectAnswer,
                        CreateAt = sa.PracticeQuestion?.CreateAt ?? sa.Question?.CreateAt,
                        Answers = sa.PracticeQuestion?.AnswerPracticeQuestions.Select(a => new
                        {
                            a.AnswerQuestionId,
                            a.AnswerText,
                            a.Number
                        }).ToList() ?? sa.Question?.AnswerQuestions.Select(a => new
                        {
                            a.AnswerQuestionId,
                            a.AnswerText,
                            a.Number
                        }).ToList()
                    }
                }).ToList();

                return Ok(data);
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
