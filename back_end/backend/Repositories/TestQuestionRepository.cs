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

        public async Task AddTestQuestions(List<TestQuestion> testQuestions)
        {
            await _testQuestionDAO.AddTestQuestions(testQuestions);
        }

        public Task<List<int>> GetQuestionIdsByTestIdAsync(int testId)
        {
            return _testQuestionDAO.GetQuestionIdsByTestIdAsync(testId);
        }

        public async Task<List<TestQuestion>> GetQuestionsByTestIdAsync(int testId)
        {
            return await _testQuestionDAO.GetQuestionsByTestIdAsync(testId);
        }

        public async Task RemoveQuestionsFromTestAsync(int testId, List<int> testQuestionsToRemove)
        {
            await _testQuestionDAO.RemoveQuestionsFromTestAsync(testId, testQuestionsToRemove);
        }
    }
}
