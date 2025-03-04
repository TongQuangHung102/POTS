using backend.Models;

namespace backend.Repositories
{
    public interface ITestQuestionRepository
    {
        Task<List<TestQuestion>> GetQuestionsByTestIdAsync(int testId);
    }
}
