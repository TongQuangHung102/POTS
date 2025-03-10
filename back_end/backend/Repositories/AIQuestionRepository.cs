using backend.DataAccess.DAO;
using backend.Models;

namespace backend.Repositories
{
    public class AIQuestionRepository : IAIQuestionRepository
    {
        private readonly AIQuestionDAO _aiQuestionDAO;

        public AIQuestionRepository(AIQuestionDAO aiQuestionDAO)
        {
            _aiQuestionDAO = aiQuestionDAO;
        }

        public async Task SaveAIQuestionAsync(AIQuestion aiQuestion)
        {
            await _aiQuestionDAO.AddAIQuestionAsync(aiQuestion);
        }
        public async Task SaveAnswerAsync(AnswerQuestion answerQuestion)
        {
            await _aiQuestionDAO.AddAIAnswerQuestionsAsync(answerQuestion);
        }
    }
}
