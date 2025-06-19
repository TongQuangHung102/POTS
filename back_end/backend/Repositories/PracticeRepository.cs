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

        public void DeletePracticeAttempt(PracticeAttempt attempt)
        {
            _attemptDAO.DeletePracticeAttempt(attempt); 
        }

        public async Task<double> GetAveragePracticeScoreAsync(int userId, int subjectGradeId, int month, int year)
        {
            return await _attemptDAO.GetAveragePracticeScoreAsync(userId, subjectGradeId, month, year);
        }

        public async Task<double> GetAveragePracticeTimeAsync(int userId, int subjectGradeId, int month, int year)
        {
            return await _attemptDAO.GetAveragePracticeTimeAsync(userId, subjectGradeId, month, year);
        }

        public async Task<double> GetAverageScoreByDateAsync(int subjectGradeId, int userId, DateTime date)
        {
            return await _attemptDAO.GetAverageScoreByDateAsync(userId, date, subjectGradeId);
        }

        public async Task<double> GetAverageTimeByDateAsync(int subjectGradeId, int userId, DateTime date)
        {
            return await _attemptDAO.GetAverageTimeByDateAsync(userId, date, subjectGradeId);
        }

        public async Task<PracticeAttempt> GetLatestAttemptAsync(int studentId, int lessonId)
        {
          return await _attemptDAO.GetLatestAttemptAsync(studentId, lessonId);
        }

        public async Task<List<PracticeAttempt>> GetOldPracticeAttemptsAsync()
        {
            return await _attemptDAO.GetOldPracticeAttemptsAsync();
        }

        public async Task<PracticeAttempt> GetPracticeAttemptDetailAsync(int practiceId)
        {
            return await _attemptDAO.GetPracticeAttemptDetailAsync(practiceId);
        }

        public async Task<(List<PracticeAttempt> Attempts, int TotalCount)> GetPracticeAttemptsByLessonAndUserAsync(int lessonId, int userId, int pageNumber, int pageSize)
        {
            return await _attemptDAO.GetPracticeAttemptsByLessonAndUserAsync(lessonId, userId, pageNumber, pageSize);
        }

        public async Task<List<(int UserId, double AverageScore, double TotalPracticeTime)>> GetStudentDataAsync(int gradeId)
        {
            return await _attemptDAO.GetStudentDataAsync(gradeId);
        }

        public async Task<int> GetTotalNumberAllPracticeAsync(int userId, DateTime date)
        {
            return await _attemptDAO.GetTotalNumberAllPracticeAsync(userId, date);
        }

        public async Task<int> GetTotalNumberPracticeAsync(int userId, int subjectGradeId, int month, int year)
        {
            return await _attemptDAO.GetTotalNumberPracticeAsync(userId, subjectGradeId, month, year);
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

        public async Task SaveChangesAsync()
        {
            await _attemptDAO.SaveChangesAsync();
        }
    }
}
