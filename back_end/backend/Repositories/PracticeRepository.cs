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

        public async Task<int> AddPracticeAttempt(PracticeAttempt practiceAttempt)
        {
            return await _attemptDAO.AddPracticeAttempt(practiceAttempt);
        }

        public async Task<double> GetAveragePracticeScoreAsync(int subjectGradeId, int userId)
        {
            return await _attemptDAO.GetAveragePracticeScoreAsync(userId, subjectGradeId);
        }

        public async Task<double> GetAveragePracticeTimeAsync(int subjectGradeId, int userId)
        {
            return await _attemptDAO.GetAveragePracticeTimeAsync(userId, subjectGradeId);
        }

        public async Task<double> GetAverageScoreByDateAsync(int subjectGradeId, int userId, DateTime date)
        {
            return await _attemptDAO.GetAverageScoreByDateAsync(userId, date, subjectGradeId);
        }

        public async Task<double> GetAverageTimeByDateAsync(int subjectGradeId, int userId, DateTime date)
        {
            return await _attemptDAO.GetAverageTimeByDateAsync(userId, date, subjectGradeId);
        }

        public async Task<PracticeAttempt> GetLastAttempt(int userId, int lessonId)
        {
          return await _attemptDAO.GetLastAttempt(userId, lessonId);
        }

        public async Task<List<(int UserId, double AverageScore, double TotalPracticeTime)>> GetStudentDataAsync(int gradeId)
        {
            return await _attemptDAO.GetStudentDataAsync(gradeId);
        }

        public async Task<int> GetTotalNumberPracticeAsync(int subjectGradeId, int userId)
        {
            return await _attemptDAO.GetTotalNumberPracticeAsync(userId, subjectGradeId);
        }

        public async Task<double> GetTotalPracticeTimeAllStudentByDateAsync(DateTime date, int? gradeId = null, int? subjectGradeId = null)
        {
            return await _attemptDAO.GetTotalPracticeTimeAllStudentByDateAsync(date, gradeId, subjectGradeId);
        }

        public async Task<double> GetTotalPracticeTimeByDateAsync(int subjectGradeId, int userId, DateTime date)
        {
            return await _attemptDAO.GetTotalPracticeTimeByDateAsync(subjectGradeId,userId, date);
        }

        public async Task<List<PracticeAttempt>> GetUserAttemptsAsync(int userId, int lessonId)
        {
          return await _attemptDAO.GetUserAttemptsAsync(userId, lessonId);
        }


    }
}
