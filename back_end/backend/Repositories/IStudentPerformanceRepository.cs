using backend.Models;

namespace backend.Repositories
{
    public interface IStudentPerformanceRepository
    {
        Task<StudentPerformance> GetOrCreateStudentPerformanceAsync(int userId, int lessonId);
        Task UpdateStudentPerformanceAsync(StudentPerformance studentPerformance);
        Task<List<StudentPerformance>> GetStudentPerformanceAsync(int studentId);
        Task<List<StudentPerformance>> GetAverageStudentPerformanceByLevel();
        
        }
}
