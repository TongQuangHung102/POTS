using backend.DataAccess.DAO;
using backend.Models;

namespace backend.Repositories
{
    public class QuestionRepository : IQuestionRepository
    {
        private readonly QuestionDAO _questionDAO;

        public QuestionRepository(QuestionDAO questionDAO)
        {
            _questionDAO = questionDAO;
        }

        public async Task<List<Question>> GetAllQuestionsAsync(int? lessonId, int? levelId, bool? isVisible, int page, int pageSize)
        {
            return await _questionDAO.GetAllQuestionsAsync(lessonId, levelId, isVisible, page, pageSize);
        }

        public async Task<int> GetTotalQuestionsAsync(int? lessonId, int? levelId, bool? isVisible)
        {
            return await _questionDAO.GetTotalQuestionsAsync(lessonId, levelId, isVisible);
        }
        public async Task<Question?> GetQuestionByIdAsync(int questionId)
        {
            return await _questionDAO.GetQuestionByIdAsync(questionId);
        }
        public async Task UpdateQuestionAsync(Question question)
        {
            await _questionDAO.UpdateQuestionAsync(question);
        }
        public async Task<int> AddQuestionAsync(Question question)
        {
            return await _questionDAO.AddQuestionAsync(question);
        }

        public async Task AddAnswerQuestionsAsync(List<AnswerQuestion> answers)
        {
            await _questionDAO.AddAnswerQuestionsAsync(answers);
        }

    }
}
