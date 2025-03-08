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

        public Task<PracticeAttempt> GetLastAttempt(int userId, int lessonId)
        {
          return _attemptDAO.GetLastAttempt(userId, lessonId);
        }

        public async Task<List<PracticeAttempt>> GetUserAttemptsAsync(int userId, int lessonId)
        {
          return await _attemptDAO.GetUserAttemptsAsync(userId, lessonId);
        }


    }
}
