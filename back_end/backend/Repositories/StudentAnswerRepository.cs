using backend.DataAccess.DAO;
using backend.Models;

namespace backend.Repositories
{
    public class StudentAnswerRepository : IStudentAnswerRepository
    {
        private readonly StudentAnswerDAO _studentAnswerDAO;

        public StudentAnswerRepository(StudentAnswerDAO studentAnswerDAO)
        {
            _studentAnswerDAO = studentAnswerDAO;
        }

        public Task<StudentAnswer> CreateAsync(StudentAnswer studentAnswer)
        {
            return _studentAnswerDAO.CreateAsync(studentAnswer);
        }

        public Task<bool> DeleteAsync(int answerId)
        {
           return _studentAnswerDAO.DeleteAsync(answerId);
        }

        public Task<List<StudentAnswer>> GetByPracticeIdAsync(int practiceId)
        {
            return _studentAnswerDAO.GetByPracticeIdAsync(practiceId);
        }
    }
}
