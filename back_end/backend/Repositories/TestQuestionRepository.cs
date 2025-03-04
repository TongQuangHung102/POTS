using backend.DataAccess.DAO;
using backend.Models;

namespace backend.Repositories
{
    public class TestQuestionRepository : ITestQuestionRepository
    {
        private readonly TestQuestionDAO _testQuestionDAO;

        public TestQuestionRepository(TestQuestionDAO testQuestionDAO)
        {
            _testQuestionDAO = testQuestionDAO;
        }

        public async Task<List<TestQuestion>> GetQuestionsByTestIdAsync(int testId)
        {
            return await _testQuestionDAO.GetQuestionsByTestIdAsync(testId);
        }
    }
}
