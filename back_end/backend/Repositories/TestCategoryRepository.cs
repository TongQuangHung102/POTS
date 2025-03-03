using backend.DataAccess.DAO;
using backend.Models;

namespace backend.Repositories
{
    public class TestCategoryRepository : ITestCategoryRepository
    {
        private readonly TestCategoryDAO _testCategoryDAO;

        public TestCategoryRepository(TestCategoryDAO testCategoryDAO)
        {
            _testCategoryDAO = testCategoryDAO;
        }

        public async Task AddTestCategoryAsync(TestCategory category)
        {
            await _testCategoryDAO.AddTestCategoryAsync(category);
        }

        public Task<List<TestCategory>> GetAllAsync()
        {
            return _testCategoryDAO.GetAllAsync();
        }

        public Task<TestCategory> GetByIdAsync(int id)
        {
            return _testCategoryDAO.GetByIdAsync(id);
        }

        public Task UpdateTestCategoryAsync(TestCategory category)
        {
            return _testCategoryDAO.UpdateTestCategoryAsync(category);
        }
    }
}
