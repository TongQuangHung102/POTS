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
        public async Task<(List<AIQuestion>, int)> GetAIQuestionsByFilters(int lessonId, int? levelId, string? status, DateTime? createdAt, int pageNumber, int pageSize)
        {
            return await _aiQuestionDAO.GetAIQuestionsByFilters(lessonId, levelId, status, createdAt, pageNumber, pageSize);
        }
        public async Task<bool> UpdateLessonIdAsync(int lessonId, List<int> aiQuestionIds)
        {
            return await _aiQuestionDAO.UpdateLessonIdAsync(lessonId, aiQuestionIds);
        }

        public async Task<int> CountQuestionAIInGrade(int gradeId)
        {
            return await _aiQuestionDAO.CountQuestionAIInGrade(gradeId);
        }
    }
}
