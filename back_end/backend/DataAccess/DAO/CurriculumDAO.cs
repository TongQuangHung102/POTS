using backend.Dtos;
using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.DataAccess.DAO
{
    public class CurriculumDAO
    {
        private readonly MyDbContext _context;

        public CurriculumDAO(MyDbContext context)
        {
            _context = context;
        }

        public async Task AddChaptersAsync(List<Chapter> chapters)
        {
            await _context.Chapters.AddRangeAsync(chapters);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Chapter>> GetAllChapterAsync(int id)
        {
            return await _context.Chapters.Include(m => m.User).Include(ls => ls.Lessons)
                .Where(g => g.SubjectGradeId == id)
                .ToListAsync();
        }
        public async Task<Chapter> GetChapterByIdAsync(int id)
        {
            return await _context.Chapters.Include(l => l.Lessons).FirstOrDefaultAsync(ch => ch.ChapterId == id);
        }

        public async Task UpdateChapterAsync(Chapter chapter)
        {
            _context.Chapters.Update(chapter);
            await _context.SaveChangesAsync();
        }

        public async Task AddLessonsAsync(List<Lesson> lessons)
        {
            await _context.Lessons.AddRangeAsync(lessons);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Lesson>> GetAllLessonAsync()
        {
            return await _context.Lessons.ToListAsync();
        }
        public async Task<Lesson> GetLessonByIdAsync(int id)
        {
            return await _context.Lessons.FirstOrDefaultAsync(ls => ls.LessonId == id);
        }
        public async Task<Lesson> GetLessonWithQuestionsByIdAsync(int id)
        {
            return await _context.Lessons
                .Include(ls => ls.Questions)
                .FirstOrDefaultAsync(ls => ls.LessonId == id);
        }

        public async Task<Lesson> GetLessonWithQuestionsAIByIdAsync(int id)
        {
            return await _context.Lessons
                .Include(ls => ls.AIQuestions)
                .FirstOrDefaultAsync(ls => ls.LessonId == id);
        }

        public async Task<List<Lesson>> GetLessonByChapterIdAsync(int id)
        {
            return await _context.Lessons.Where(ls => ls.ChapterId == id).ToListAsync();
        }

        public async Task UpdateLessonAsync(Lesson lesson)
        {
            _context.Lessons.Update(lesson);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Chapter>> GetChaptersByIdsAsync(List<int> chapterIds)
        {
            return await _context.Chapters.Where(ch => chapterIds.Contains(ch.ChapterId)).ToListAsync();
        }

        public async Task UpdateChaptersAsync(List<Chapter> chapters)
        {
            _context.Chapters.UpdateRange(chapters);
            await _context.SaveChangesAsync();
        }
        public async Task<List<Chapter>> GetChaptersWithQuestionsBySubjectGradeAsync(int subjectGradeId)
        {
            return await _context.Chapters
                .Where(c => c.SubjectGradeId == subjectGradeId)
                .Include(c => c.Lessons)
                    .ThenInclude(l => l.Questions)
                .ToListAsync();
        }




    }
}
