using backend.Models;

namespace backend.DataAccess.DAO
{
    public class StudentTestDAO
    {
        private readonly MyDbContext _dbContext;

        public StudentTestDAO(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddStudentTest(StudentTest studentTest)
        {
            _dbContext.StudentTests.Add(studentTest);
            await _dbContext.SaveChangesAsync();
        }
    }
}
