using backend.Models;

namespace backend.Repositories
{
    public interface ITestCategoryRepository
    {
        Task<List<TestCategory>> GetAllAsync();
        Task<TestCategory> GetByIdAsync(int id);
        Task AddTestCategoryAsync(TestCategory category);
        Task UpdateTestCategoryAsync(TestCategory category);
    }
}
