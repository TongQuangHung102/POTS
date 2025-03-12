using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.DataAccess.DAO
{
    public class PracticeAttemptDAO
    {
        private readonly MyDbContext _context;

        public PracticeAttemptDAO(MyDbContext context)
        {
            _context = context;
        }

        public async Task<List<PracticeAttempt>> GetUserAttemptsAsync(int userId, int lessonId)
        {
            return await _context.PracticeAttempts
            .Where(a => a.UserId == userId && a.LessonId == lessonId)
            .ToListAsync();
        }

        public async Task AddPracticeAttemp(PracticeAttempt practiceAttempt)
        {
             _context.PracticeAttempts.AddAsync(practiceAttempt);
            await _context.SaveChangesAsync();
        } 

        public async Task<PracticeAttempt> GetLastAttempt(int userId, int lessonId)
        {
            return await _context.PracticeAttempts
                .Where(a => a.UserId == userId && a.LessonId == lessonId)
                .OrderByDescending(a => a.PracticeId)
                .FirstOrDefaultAsync();
        }
        public async Task<double> GetTotalPracticeTimeByDateAsync(int userId, DateTime date)
        {
            var totalMinutes = await _context.PracticeAttempts
                .Where(p => p.UserId == userId && p.CreateAt.Date == date.Date)
                .Select(p => p.TimePractice)
                .SumAsync();

            return totalMinutes;
        }

        public async Task<int> GetTotalNumberPracticeAsync(int userId)
        {
            return await _context.PracticeAttempts
                .Where(p => p.UserId == userId)
                .CountAsync();
        }

        public async Task<double> GetAveragePracticeTimeAsync(int userId)
        {
            return await _context.PracticeAttempts
                .Where(u => u.UserId == userId)
                .Select(p => p.TimePractice)
                .AverageAsync();
        }

        public async Task<double> GetAveragePracticeScoreAsync(int userId)
        {
            return await _context.PracticeAttempts
                .Where(u => u.UserId == userId)
                .Select(p => p.CorrectAnswers)
                .AverageAsync();
        }

        public async Task<List<(int UserId, double AverageScore, double TotalPracticeTime)>> GetStudentDataAsync(int gradeId)
        {
            var studentData = await _context.Users
                .Where(u => u.GradeId == gradeId)
                .Select(u => new
                {
                    UserId = u.UserId,
                    AverageScore = _context.PracticeAttempts
                        .Where(p => p.UserId == u.UserId)
                        .Select(p => (double)p.CorrectAnswers / 10)
                        .DefaultIfEmpty(0)
                        .Average(),
                    TotalPracticeTime = _context.PracticeAttempts
                        .Where(p => p.UserId == u.UserId)
                        .Select(p => p.TimePractice)
                        .DefaultIfEmpty(0)
                        .Sum()
                })
                .ToListAsync();

            return studentData.Select(s => (s.UserId, s.AverageScore, s.TotalPracticeTime)).ToList();
        }



    }
}
