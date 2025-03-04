using backend.Models;

namespace backend.Repositories
{
    public interface ITestQuestionRepository
    {
        Task AddTestQuestions(List<TestQuestion> testQuestions);
        Task<List<Question>> GetQuestionsByTestIdAsync(int testId);
        Task<List<int>> GetQuestionIdsByTestIdAsync(int testId);
        Task RemoveQuestionsFromTestAsync(int testId, List<int> testQuestionsToRemove);
    }
}
