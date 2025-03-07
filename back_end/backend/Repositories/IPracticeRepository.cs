using backend.Models;

namespace backend.Repositories
{
    public interface IPracticeRepository
    {
        Task<List<PracticeAttempt>> GetUserAttemptsAsync(int userId, int lessonId);
         Task AddPracticeAttemp(PracticeAttempt practiceAttempt);
        Task<StudentPerformance> GetOrCreateStudentPerformanceAsync(int userId, int lessonId);
        Task UpdateStudentPerformanceAsync(StudentPerformance studentPerformance);
    }
}
