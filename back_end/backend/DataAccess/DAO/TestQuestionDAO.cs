using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.DataAccess.DAO
{
    public class TestQuestionDAO
    {

        private readonly MyDbContext _context;

        public TestQuestionDAO(MyDbContext context)
        {
            _context = context;
        }


        public async Task AddTestQuestions(List<TestQuestion> testQuestions)
        {
            _context.TestQuestions.AddRange(testQuestions);
            await _context.SaveChangesAsync();
        }
        public async Task<List<TestQuestion>> GetQuestionsByTestIdAsync(int testId)
        {
            return await _context.TestQuestions
            .Where(tq => tq.TestId == testId)
              .Include(tq => tq.Test)
            .Include(tq => tq.Question)
                .ThenInclude(q => q.Level)
            .Include(tq => tq.Question)
                .ThenInclude(q => q.Lesson)
            .Include(tq => tq.Question)
                .ThenInclude(q => q.AnswerQuestions)
            .ToListAsync();
        }

        //  Lấy id cua cac cau hoi trong bai test
        public async Task<List<int>> GetQuestionIdsByTestIdAsync(int testId)
        {
            return await _context.TestQuestions
                .Where(tq => tq.TestId == testId)
                .Select(tq => tq.QuestionId)
                .ToListAsync();
        }

        // xoa cac cau hoi khoi bai test
        public async Task RemoveQuestionsFromTestAsync(int testId, List<int> questionIds)
        {
            var testQuestionsToRemove = await _context.TestQuestions
                .Where(tq => tq.TestId == testId && questionIds.Contains(tq.QuestionId))
                .ToListAsync();

            _context.TestQuestions.RemoveRange(testQuestionsToRemove);
            await _context.SaveChangesAsync();
        }

    }
}
