using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.DataAccess.DAO
{
    public class StudentPerformanceDAO
    {
        private readonly MyDbContext _context;

        public StudentPerformanceDAO(MyDbContext context)
        {
            _context = context;
        }

        public async Task<StudentPerformance> GetOrCreateStudentPerformanceAsync(int userId, int lessonId)
        {
            var currentMonth = DateTime.UtcNow.Month;
            var currentYear = DateTime.UtcNow.Year;

            var studentPerformance = await _context.StudentPerformances
                .FirstOrDefaultAsync(sp => sp.UserId == userId && sp.LessonId == lessonId && sp.Month == currentMonth && sp.Year == currentYear);

            if (studentPerformance == null)
            {
                studentPerformance = new StudentPerformance
                {
                    UserId = userId,
                    LessonId = lessonId,
                    avg_Accuracy = 0,
                    avg_Time = 0,
                    LastAttempt = DateTime.UtcNow,
                    LevelId = 1,  
                    AttemptCount = 0,
                    Month = currentMonth,
                    Year = currentYear
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

        public async Task<List<StudentPerformance>> GetStudentPerformanceAsync(int studentId)
        {
            return await _context.StudentPerformances
                .Where(sp => sp.UserId == studentId)
                .ToListAsync();
        }

        public async Task<List<StudentPerformance>> GetAverageStudentPerformance(int month, int year)
        {
            return await _context.StudentPerformances
                .Where(sp => sp.Month == month && sp.Year == year) 
                .GroupBy(sp => sp.UserId)
                .Select(g => new StudentPerformance
                {
                    UserId = g.Key,
                    avg_Accuracy = g.Average(sp => sp.avg_Accuracy ?? 0),
                    avg_Time = g.Average(sp => sp.avg_Time ?? 0),
                    AttemptCount = g.Sum(sp => sp.AttemptCount)
                })
                .ToListAsync();
        }


    }
}
