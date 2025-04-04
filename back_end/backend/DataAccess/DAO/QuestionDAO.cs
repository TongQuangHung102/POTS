﻿using backend.Dtos.Dashboard;
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

        public async Task<List<Question>> GetQuestionsFirstTimePractice(int count, int lessonId)
        {
            return await _context.Questions
                .Where(q => q.LevelId == 2 && q.IsVisible && q.LessonId == lessonId) 
                .OrderBy(q => Guid.NewGuid()) 
                .Take(count)
                .Include(q => q.Level)
                .Include(q => q.Lesson)
                .Include(q => q.AnswerQuestions)
                .ToListAsync();
        }

        public async Task<List<Question>> GetQuestionsPractice(int count, int lessonId, int levelId)
        {
            return await _context.Questions
                .Where(q => q.LevelId == levelId && q.IsVisible && q.LessonId == lessonId)
                .OrderBy(q => Guid.NewGuid())
                .Take(count)
                .Include(q => q.Level)
                .Include(q => q.Lesson)
                .Include(q => q.AnswerQuestions)
                .ToListAsync();
        }

        public async Task<int> CountQuestionInSubjectGrade(int id)
        {
            return await _context.Questions
                .Where(q => q.Lesson.Chapter.SubjectGrade.GradeId == id) 
                .CountAsync();
        }

        public async Task<List<Question>> GetQuestionsByChapterAutoAsync(ChapterQuestionAutoRequest chapterRequest)
        {
            var questions = new List<Question>();

            var lessonIds = await _context.Lessons
                .Where(l => l.ChapterId == chapterRequest.ChapterId)
                .Select(l => l.LessonId)
                .ToListAsync();

            foreach (var level in chapterRequest.LevelRequests)
            {
                var levelQuestions = await _context.Questions
                    .Where(q => lessonIds.Contains(q.LessonId) && q.LevelId == level.LevelId)
                    .OrderBy(q => Guid.NewGuid()) 
                    .Take(level.QuestionCount)
                    .Include(q => q.Level)
                    .Include(q => q.Lesson)
                    .Include(q => q.AnswerQuestions)
                    .ToListAsync();

                questions.AddRange(levelQuestions);
            }

            return questions;
        }

        public async Task MarkQuestionsAsUsed(List<int> questionIds)
        {
            var questions = await _context.Questions
                                          .Where(q => questionIds.Contains(q.QuestionId))
                                          .ToListAsync();

            foreach (var question in questions)
            {
                question.IsUsed = true;
                question.UsedAt = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        public async Task<int> CountQuestionsUsedByWeek(int subjectGradeId, int weekOffset)
        {
            var today = DateTime.UtcNow.Date;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek).AddDays(-7 * weekOffset);
            var endOfWeek = startOfWeek.AddDays(7);

            var count = await _context.Questions
                .Where(q => q.Lesson.Chapter.SubjectGradeId == subjectGradeId
                            && q.IsUsed == true
                            && q.UsedAt >= startOfWeek
                            && q.UsedAt < endOfWeek)
                .CountAsync();

            return count;
        }


    }
}
