using backend.DataAccess.DAO;
using backend.Models;

namespace backend.Repositories
{
    public class TestRepository : ITestRepository
    {
        private readonly TestDAO _testDAO;

        public TestRepository(TestDAO testDAO)
        {
            _testDAO = testDAO;
        }

        public async Task<List<Test>> GetAllAsync()
        {
            return await _testDAO.GetAllAsync();
        }

        public async Task<Test> GetByIdAsync(int id)
        {
            return await _testDAO.GetByIdAsync(id);
        }

        public async Task AddAsync(Test test)
        {
            await _testDAO.AddAsync(test);
        }

        public async Task UpdateAsync(Test test)
        {
            await _testDAO.UpdateAsync(test);
        }
        public async Task<List<Test>> GetTestsByGradeIdAsync(int gradeId)
        {
            return await _testDAO.GetTestsByGradeIdAsync(gradeId);
        }

    }
}
