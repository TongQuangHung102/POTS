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

        public async Task<List<TestQuestion>> GetQuestionsByTestIdAsync(int testId)
        {
            return await _context.TestQuestions
                .Where(tq => tq.TestId == testId)
                .Include(tq => tq.Test) 
                .Include(tq => tq.Question) 
                .ThenInclude(q => q.AnswerQuestions)  
                .ToListAsync();
        }
    }
}
