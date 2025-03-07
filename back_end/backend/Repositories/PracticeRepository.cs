using backend.DataAccess.DAO;
using backend.Models;

namespace backend.Repositories
{
    public class PracticeRepository : IPracticeRepository
    {
        private readonly PracticeAttemptDAO _attemptDAO;

        public PracticeRepository(PracticeAttemptDAO attemptDAO)
        {
            _attemptDAO = attemptDAO;
        }

        public async Task AddPracticeAttemp(PracticeAttempt practiceAttempt)
        {
            await _attemptDAO.AddPracticeAttemp(practiceAttempt);
        }

        public async Task<StudentPerformance> GetOrCreateStudentPerformanceAsync(int userId, int lessonId)
        {
            return await _attemptDAO.GetOrCreateStudentPerformanceAsync(userId, lessonId);
        }

        public async Task<List<PracticeAttempt>> GetUserAttemptsAsync(int userId, int lessonId)
        {
          return await _attemptDAO.GetUserAttemptsAsync(userId, lessonId);
        }

        public async Task UpdateStudentPerformanceAsync(StudentPerformance studentPerformance)
        {
            await _attemptDAO.UpdateStudentPerformanceAsync(studentPerformance);
        }
    }
}
