using backend.Models;

namespace backend.Repositories
{
    public interface ITestRepository
    {
        Task<List<Test>> GetAllAsync();
        Task<Test> GetByIdAsync(int id);
        Task AddAsync(Test test);
        Task UpdateAsync(Test test);
        Task<List<Test>> GetTestBySubjectGradeIdAsync(int id);

    }
}
