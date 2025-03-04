using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.DataAccess.DAO
{
    public class TestQuestionDAO
    {
        private readonly MyDbContext _dbContext;

        public TestQuestionDAO(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddTestQuestions(List<TestQuestion> testQuestions)
        {
            _dbContext.TestQuestions.AddRange(testQuestions);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<List<Question>> GetQuestionsByTestIdAsync(int testId)
        {
            return await _dbContext.TestQuestions
            .Where(tq => tq.TestId == testId)
            .Include(tq => tq.Question)
                .ThenInclude(q => q.Level)
            .Include(tq => tq.Question)
                .ThenInclude(q => q.Lesson)
            .Include(tq => tq.Question)
                .ThenInclude(q => q.AnswerQuestions)
            .Select(tq => tq.Question)
            .ToListAsync();
        }

        //  Lấy id cua cac cau hoi trong bai test
        public async Task<List<int>> GetQuestionIdsByTestIdAsync(int testId)
        {
            return await _dbContext.TestQuestions
                .Where(tq => tq.TestId == testId)
                .Select(tq => tq.QuestionId)
                .ToListAsync();
        }

        // xoa cac cau hoi khoi bai test
        public async Task RemoveQuestionsFromTestAsync(int testId, List<int> questionIds)
        {
            var testQuestionsToRemove = await _dbContext.TestQuestions
                .Where(tq => tq.TestId == testId && questionIds.Contains(tq.QuestionId))
                .ToListAsync();

            _dbContext.TestQuestions.RemoveRange(testQuestionsToRemove);
            await _dbContext.SaveChangesAsync();
        }
    }
}
