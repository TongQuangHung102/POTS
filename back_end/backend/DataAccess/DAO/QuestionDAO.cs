using backend.Models;
using MailKit.Search;
using Microsoft.EntityFrameworkCore;

namespace backend.DataAccess.DAO
{
    public class QuestionDAO
    {
        private readonly MyDbContext _context;

        public QuestionDAO(MyDbContext context)
        {
            _context = context;
        }

        public async Task<List<Question>> GetAllQuestionsAsync(int? chapterId, int? lessonId, int? levelId, string searchTerm, bool? isVisible, int page, int pageSize)
        {
            if (page < 1) page = 1;

            int skip = (page - 1) * pageSize;

            var query = _context.Questions
                .Include(q => q.Level)
                .Include(q => q.Lesson)
                    .ThenInclude(l => l.Chapter)
                .Include(q => q.AnswerQuestions)
                .Include(q => q.ContestQuestions)
                .Include(q => q.TestQuestions)
                .Include(q => q.StudentAnswers)
                .AsQueryable();

            if (chapterId.HasValue)
            {
                query = query.Where(q => q.Lesson.ChapterId == chapterId.Value);
            }

            if (levelId.HasValue)
            {
                query = query.Where(q => q.LevelId == levelId.Value);
            }

            if (lessonId.HasValue)
            {
                query = query.Where(q => q.LessonId == lessonId.Value);
            }

            if (isVisible.HasValue)
            {
                query = query.Where(q => q.IsVisible == isVisible.Value);
            }

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(q => q.QuestionText.ToLower().Contains(searchTerm.ToLower()));

            }
            return await query
                .OrderBy(q => q.QuestionId)
                .Skip(skip)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalQuestionsAsync(int? chapterId, int? lessonId,int? levelId, string searchTerm, bool? isVisible)
        {
            var query = _context.Questions.AsQueryable();

            if (chapterId.HasValue)
            {
                query = query.Where(q => q.Lesson.ChapterId == chapterId.Value);
            }

            if (levelId.HasValue)
            {
                query = query.Where(q => q.LevelId == levelId.Value);
            }

            if (lessonId.HasValue)
            {
                query = query.Where(q => q.LessonId == lessonId.Value);
            }

            if (isVisible.HasValue)
            {
                query = query.Where(q => q.IsVisible == isVisible.Value);
            }
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(q => q.QuestionText.ToLower().Contains(searchTerm.ToLower()));

            }

            return await query.CountAsync();
        }

        public async Task<Question?> GetQuestionByIdAsync(int questionId)
        {
            return await _context.Questions
                .Include(q => q.Level)
                .Include(q => q.Lesson)
                .Include(q => q.AnswerQuestions) 
                .FirstOrDefaultAsync(q => q.QuestionId == questionId);
        }

        public async Task UpdateQuestionAsync(Question question)
        {
            _context.Questions.Update(question);
            await _context.SaveChangesAsync();
        }
        public async Task<int> AddQuestionAsync(Question question)
        {
            await _context.Questions.AddAsync(question);
            await _context.SaveChangesAsync();
            return question.QuestionId;
        }

        public async Task AddAnswerQuestionsAsync(List<AnswerQuestion> answers)
        {
            await _context.AnswerQuestions.AddRangeAsync(answers);
            await _context.SaveChangesAsync();
        }


    }
}
