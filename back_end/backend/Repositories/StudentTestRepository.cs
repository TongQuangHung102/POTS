using backend.DataAccess.DAO;
using backend.Models;

namespace backend.Repositories
{
    public class StudentTestRepository : IStudentTestRepository
    {
        private readonly StudentTestDAO _studentTestDAO;

        public StudentTestRepository(StudentTestDAO studentTestDAO)
        {
            _studentTestDAO = studentTestDAO;
        }

        public async Task AddStudentTest(StudentTest studentTest)
        {
            await _studentTestDAO.AddStudentTest(studentTest);
        }
    }
}
