
using backend.DataAccess.DAO;
using backend.Dtos.Questions;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class TestQuestionService
    {
        private readonly ITestQuestionRepository _testQuestionRepository;

        public TestQuestionService(ITestQuestionRepository testQuestionRepository)
        {
            _testQuestionRepository = testQuestionRepository;
        }

        public async Task<IActionResult> AddQuestionsToTest(AddQuestionsToTestDto dto)
        {
            var testQuestions = dto.QuestionIds.Select(qId => new TestQuestion
            {
                TestId = dto.TestId,
                QuestionId = qId
            }).ToList();

            try
            {
                await _testQuestionRepository.AddTestQuestions(testQuestions);
                return new OkObjectResult(new
                {
                    Message = "Thêm mới thành công!"
                });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { Message = "Lỗi khi thêm mới.", Error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }

        public async Task<IActionResult> GetTestQuestionsAsync(int testId)
        {
            var questions = await _testQuestionRepository.GetQuestionsByTestIdAsync(testId);

            if (questions == null || !questions.Any())
            {
                return new NotFoundObjectResult(new { message = "Chưa có câu hỏi nào được thêm vào bài kiểm tra." });
            }

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

            return new OkObjectResult(response);
        }

        public async Task<IActionResult> UpdateTestQuestionsAsync(AddQuestionsToTestDto dto)
        {
            if (dto.TestId <= 0 || dto.QuestionIds == null)
            {
                return new NotFoundObjectResult(new { message = "Không tìm thấy bài kiểm tra này." });
            }

            var existingQuestionIds = await _testQuestionRepository.GetQuestionIdsByTestIdAsync(dto.TestId);


            var questionsToAdd = dto.QuestionIds.Except(existingQuestionIds).ToList();
            var testQuestions = questionsToAdd.Select(qId => new TestQuestion
            {
                TestId = dto.TestId,
                QuestionId = qId
            }).ToList();

            var questionsToRemove = existingQuestionIds.Except(dto.QuestionIds).ToList();

            if (questionsToAdd.Any())
            {
                await _testQuestionRepository.AddTestQuestions(testQuestions);
            }

            if (questionsToRemove.Any())
            {
                await _testQuestionRepository.RemoveQuestionsFromTestAsync(dto.TestId, questionsToRemove);
            }

            return new OkObjectResult(new
            {
                Message = "Cập nhật thành công"
            });
        }

    }
}
