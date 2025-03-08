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
    }
}
