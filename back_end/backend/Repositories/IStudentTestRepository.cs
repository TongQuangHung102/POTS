using backend.Models;

namespace backend.Repositories
{
    public interface IStudentTestRepository
    {
        Task AddStudentTest(StudentTest studentTest);
    }
}
