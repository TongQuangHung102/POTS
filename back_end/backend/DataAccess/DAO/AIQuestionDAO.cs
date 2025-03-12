using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.DataAccess.DAO
{
    public class AIQuestionDAO
    {
        private readonly MyDbContext _context;

        public AIQuestionDAO(MyDbContext context)
        {
            _context = context;
        }

        public async Task AddAIQuestionAsync(AIQuestion question)
        {
            _context.AIQuestions.Add(question);
            await _context.SaveChangesAsync(); 
            await _context.Entry(question).ReloadAsync();
        }
        public async Task AddAIAnswerQuestionsAsync(AnswerQuestion answer)
        {
            _context.AnswerQuestions.Add(answer);
            await _context.SaveChangesAsync();
        }
        public async Task<(List<AIQuestion>, int)> GetAIQuestionsByFilters(int lessonId, int? levelId, string? status, DateTime? createdAt, int pageNumber, int pageSize)
        {
            var query = _context.AIQuestions.AsQueryable().Where(q => q.LessonId == lessonId);

            if (levelId.HasValue)
            {
                query = query.Where(q => q.LevelId == levelId.Value);
            }

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(q => q.Status == status);
            }

            if (createdAt.HasValue)
            {
                query = query.Where(q => q.CreateAt.Date == createdAt.Value.Date);
            }

            int totalRecords = await query.CountAsync();
            int totalPages = (int)Math.Ceiling(totalRecords / (double)pageSize);

            var questions = await query
                .OrderByDescending(q => q.CreateAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .AsNoTracking()
                .Include(q => q.Level)
                .Include(q => q.Lesson)
                .ToListAsync();

            return (questions, totalPages);
        }
        public async Task<bool> UpdateLessonIdAsync(int lessonId, List<int> aiQuestionIds)
        {
            var questions = await _context.AIQuestions
                                          .Where(q => aiQuestionIds.Contains(q.QuestionId))
                                          .ToListAsync();

            if (!questions.Any())
                return false;

            foreach (var question in questions)
            {
                question.LessonId = lessonId;
            }

            await _context.SaveChangesAsync();
            return true;
        }

    }
}
