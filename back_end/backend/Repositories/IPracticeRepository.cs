using backend.Models;

namespace backend.Repositories
{
    public interface IPracticeRepository
    {
        Task<List<PracticeAttempt>> GetUserAttemptsAsync(int userId, int lessonId);
         Task AddPracticeAttemp(PracticeAttempt practiceAttempt);
        Task<PracticeAttempt> GetLastAttempt(int userId, int lessonId);
        Task<double> GetTotalPracticeTimeByDateAsync(int subjectGradeId, int userId, DateTime date);
        Task<double> GetAverageScoreByDateAsync(int subjectGradeId, int userId, DateTime date);
        Task<double> GetAverageTimeByDateAsync(int subjectGradeId, int userId, DateTime date);
        Task<int> GetTotalNumberPracticeAsync(int subjectGradeId, int userId);
        Task<double> GetAveragePracticeTimeAsync(int subjectGradeId, int userId);
        Task<double> GetAveragePracticeScoreAsync(int subjectGradeId, int userId);
        Task<List<(int UserId, double AverageScore, double TotalPracticeTime)>> GetStudentDataAsync(int gradeId);
        Task<double> GetTotalPracticeTimeAllStudentByDateAsync(DateTime date, int? gradeId = null, int? subjectGradeId = null);
    }
}
