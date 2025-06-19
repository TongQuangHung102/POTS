using backend.DataAccess.DAO;
using backend.Models;

namespace backend.Repositories
{
    public class StudentPerformanceRepository : IStudentPerformanceRepository
    {
        private readonly StudentPerformanceDAO _studentPerformanceDAO;

        public StudentPerformanceRepository(StudentPerformanceDAO studentPerformanceDAO)
        {
            _studentPerformanceDAO = studentPerformanceDAO;
        }

        public async Task<List<StudentPerformance>> GetAverageStudentPerformance(int month, int year)
        {
            return  await _studentPerformanceDAO.GetAverageStudentPerformance(month, year);
        }

        public async Task<StudentPerformance> GetOrCreateStudentPerformanceAsync(int userId, int lessonId)
        {
            return await _studentPerformanceDAO.GetOrCreateStudentPerformanceAsync(userId, lessonId);
        }

        public async Task<List<StudentPerformance>> GetStudentPerformanceAsync(int studentId)
        {
           return await _studentPerformanceDAO.GetStudentPerformanceAsync(studentId);
        }

        public async Task UpdateStudentPerformanceAsync(StudentPerformance studentPerformance)
        {
            await _studentPerformanceDAO.UpdateStudentPerformanceAsync(studentPerformance);
        }
    }
}
