using backend.Models;

namespace backend.Repositories
{
    public interface IAIQuestionRepository
    {
        Task SaveAIQuestionAsync(AIQuestion aiQuestion);

        Task SaveAnswerAsync(AnswerQuestion answerQuestion);
    }
}
