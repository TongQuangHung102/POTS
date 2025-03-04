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
    }
}
