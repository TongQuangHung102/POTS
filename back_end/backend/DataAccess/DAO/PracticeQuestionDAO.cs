using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.DataAccess.DAO
{
    public class PracticeQuestionDAO
    {
        private readonly MyDbContext _context;

        public PracticeQuestionDAO(MyDbContext context)
        {
            _context = context;
        }

        public async Task<PracticeQuestion> CreateAsync(PracticeQuestion practiceQuestion)
        {
            _context.PracticeQuestions.Add(practiceQuestion);
            await _context.SaveChangesAsync();
            return practiceQuestion;
        }

        public void DeletePracticeQuestions(List<PracticeQuestion> practiceQuestions)
        {
            _context.PracticeQuestions.RemoveRange(practiceQuestions);
        }

        public async Task AddAnswerQuestionsAsync(List<AnswerPracticeQuestion> answers)
        {
            await _context.AnswerPracticeQuestions.AddRangeAsync(answers);
            await _context.SaveChangesAsync();
        }

        public async Task<List<PracticeQuestion>> GetExpiredUnansweredQuestionsAsync(DateTime expired)
        {
            return await _context.PracticeQuestions
                .Where(pq => pq.CreateAt < expired &&
                             !_context.StudentAnswers.Any(sa => sa.PracticeQuestionId == pq.PracticeQuestionId))
                .ToListAsync();
        }

        public async Task SaveAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
