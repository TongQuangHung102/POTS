using backend.Models;

namespace backend.Repositories
{
    public interface IStudentPerformanceRepository
    {
        Task<StudentPerformance> GetOrCreateStudentPerformanceAsync(int userId, int lessonId);
        Task UpdateStudentPerformanceAsync(StudentPerformance studentPerformance);
    }
}
