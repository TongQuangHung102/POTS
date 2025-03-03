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

        public async Task<List<Question>> GetAllQuestionsAsync(int? chapterId, int? lessonId, int? levelId, string searchTerm, bool? isVisible, int page, int pageSize)
        {
            return await _questionDAO.GetAllQuestionsAsync(chapterId, lessonId, levelId, searchTerm, isVisible, page, pageSize);
        }

        public async Task<int> GetTotalQuestionsAsync(int? chapterId, int? lessonId, int? levelId, string searchTerm, bool? isVisible)
        {
            return await _questionDAO.GetTotalQuestionsAsync(chapterId, lessonId, levelId, searchTerm, isVisible);
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
