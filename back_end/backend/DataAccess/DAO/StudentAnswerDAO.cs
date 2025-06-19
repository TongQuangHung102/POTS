using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.DataAccess.DAO
{
    public class StudentAnswerDAO
    {
        private readonly MyDbContext _dbContext;

        public StudentAnswerDAO(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<StudentAnswer> CreateAsync(StudentAnswer studentAnswer)
        {
            _dbContext.StudentAnswers.Add(studentAnswer);
            await _dbContext.SaveChangesAsync();
            return studentAnswer;
        }

        public void DeleteStudentAnswers(List<StudentAnswer> studentAnswers)
        {
            _dbContext.StudentAnswers.RemoveRange(studentAnswers);
        }

        public async Task<List<StudentAnswer>> GetByPracticeIdAsync(int practiceId)
        {
            return await _dbContext.StudentAnswers
                .Where(sa => sa.PracticeId == practiceId)
                .Include(sa => sa.PracticeAttempt)  
                .Include(sa => sa.PracticeQuestion) 
                .ToListAsync();
        }
    }
}
