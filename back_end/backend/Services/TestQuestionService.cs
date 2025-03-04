
using backend.DataAccess.DAO;
using backend.Dtos;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class TestQuestionService
    {
        private readonly MyDbContext _context;

        public TestQuestionService(MyDbContext context)
        {
            _context = context;
        }

        public async Task<List<TestQuestionDto>> GetTestQuestionsByTestId(int testId)
        {
            var testQuestions = await _context.TestQuestions
                .Where(tq => tq.TestId == testId)
                .Select(tq => new TestQuestionDto
                {
                    TestQuestionId = tq.TestQuestionId,
                    TestId = tq.TestId,
                    TestName = tq.Test.TestName,
                    TestDescription = tq.Test.Description,
                    DurationInMinutes = tq.Test.DurationInMinutes,
                    QuestionId = tq.QuestionId,
                    QuestionText = tq.Question.QuestionText,
                    CorrectAnswer = tq.Question.CorrectAnswer,
                    IsVisible = tq.Question.IsVisible,
                    Answers = tq.Question.AnswerQuestions.Select(a => new AnswerQuestionDto
                    {
                        AnswerQuestionId = a.AnswerQuestionId,
                        AnswerText = a.AnswerText,
                        Number = a.Number
                    }).ToList()
                })
                .ToListAsync();

            return testQuestions;
        }

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

            var response = questions.Select(q => new
            {
                q.QuestionId,
                q.QuestionText,
                q.CreateAt,
                q.CorrectAnswer,
                CorrectAnswerText = q.AnswerQuestions.FirstOrDefault(a => a.Number == q.CorrectAnswer)?.AnswerText,
                q.IsVisible,
                q.CreateByAI,
                Level = new
                {
                    q.Level.LevelName,
                    q.Level.LevelId
                },
                Lesson = new
                {
                    q.Lesson.LessonName
                },
                AnswerQuestions = q.AnswerQuestions.Select(a => new
                {
                    a.AnswerQuestionId,
                    a.AnswerText,
                    a.Number
                }).ToList()
            });

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
