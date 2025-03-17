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

        public async Task<double> GetAveragePracticeScoreAsync(int userId)
        {
            return await _attemptDAO.GetAveragePracticeScoreAsync(userId);
        }

        public async Task<double> GetAveragePracticeTimeAsync(int userId)
        {
            return await _attemptDAO.GetAveragePracticeTimeAsync(userId);
        }

        public async Task<double> GetAverageScoreByDateAsync(int userId, DateTime date)
        {
            return await _attemptDAO.GetAverageScoreByDateAsync(userId, date);
        }

        public async Task<double> GetAverageTimeByDateAsync(int userId, DateTime date)
        {
            return await _attemptDAO.GetAverageTimeByDateAsync(userId, date);
        }

        public async Task<PracticeAttempt> GetLastAttempt(int userId, int lessonId)
        {
          return await _attemptDAO.GetLastAttempt(userId, lessonId);
        }

        public async Task<List<(int UserId, double AverageScore, double TotalPracticeTime)>> GetStudentDataAsync(int gradeId)
        {
            return await _attemptDAO.GetStudentDataAsync(gradeId);
        }

        public async Task<int> GetTotalNumberPracticeAsync(int userId)
        {
            return await _attemptDAO.GetTotalNumberPracticeAsync(userId);
        }

        public async Task<double> GetTotalPracticeTimeAllStudentByDateAsync(DateTime date, int? gradeId = null)
        {
            return await _attemptDAO.GetTotalPracticeTimeAllStudentByDateAsync(date);
        }

        public async Task<double> GetTotalPracticeTimeByDateAsync(int userId, DateTime date)
        {
            return await _attemptDAO.GetTotalPracticeTimeByDateAsync(userId, date);
        }

        public async Task<List<PracticeAttempt>> GetUserAttemptsAsync(int userId, int lessonId)
        {
          return await _attemptDAO.GetUserAttemptsAsync(userId, lessonId);
        }


    }
}
