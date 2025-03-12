using backend.Models;

namespace backend.Repositories
{
    public interface IAIQuestionRepository
    {
        Task SaveAIQuestionAsync(AIQuestion aiQuestion);

        Task SaveAnswerAsync(AnswerQuestion answerQuestion);
        Task<(List<AIQuestion>, int)> GetAIQuestionsByFilters(int lessonId, int? levelId, string? status, DateTime? createdAt, int pageNumber, int pageSize);
        Task<bool> UpdateLessonIdAsync(int lessonId, List<int> aiQuestionIds);
    }
}
