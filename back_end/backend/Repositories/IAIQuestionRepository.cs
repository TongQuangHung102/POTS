using backend.Models;

namespace backend.Repositories
{
    public interface IAIQuestionRepository
    {
        Task SaveAIQuestionAsync(AIQuestion aiQuestion);

        Task SaveAnswerAsync(AnswerQuestion answerQuestion);
        Task<(List<AIQuestion>, int)> GetAIQuestionsByFilters(int lessonId, int? levelId, string? status, DateTime? createdAt, int pageNumber, int pageSize);
        Task<bool> UpdateLessonIdAsync(int lessonId, List<int> aiQuestionIds);
        Task<int> CountQuestionAIInGrade(int gradeId); 
        Task<AIQuestion?> GetAIQuestionByIdAsync(int questionId);
        Task<bool> UpdateAIQuestionAsync(AIQuestion aiQuestion);
        Task<bool> DeleteAnswersByQuestionId(int questionId);
        Task<int> SaveQuestionAsync(Question question);
        Task<bool> UpdateAnswerAsync(AnswerQuestion answerQuestion);

    }
}
