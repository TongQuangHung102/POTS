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

        public async Task<StudentPerformance> GetOrCreateStudentPerformanceAsync(int userId, int lessonId)
        {
            var studentPerformance = await _context.StudentPerformances
                .FirstOrDefaultAsync(sp => sp.UserId == userId && sp.LessonId == lessonId);

            if (studentPerformance == null)
            {
                studentPerformance = new StudentPerformance
                {
                    UserId = userId,
                    LessonId = lessonId,
                    avg_Accuracy = 0,
                    avg_Time_Per_Question = TimeSpan.Zero,
                    LastAttempt = DateTime.UtcNow,
                    LevelId = 2
                };
                _context.StudentPerformances.Add(studentPerformance);
                await _context.SaveChangesAsync();
            }
            return studentPerformance;
        }

        public async Task UpdateStudentPerformanceAsync(StudentPerformance studentPerformance)
        {
            _context.StudentPerformances.Update(studentPerformance);
            await _context.SaveChangesAsync();
        }
    }
}
