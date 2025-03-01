using backend.Models;

namespace backend.Repositories
{
    public interface IQuestionRepository
    {
        Task<List<Question>> GetAllQuestionsAsync(int? lessonId, int? levelId, bool? isVisible, int page, int pageSize);
        Task<int> GetTotalQuestionsAsync(int? lessonId, int? levelId, bool? isVisible);
        Task<Question?> GetQuestionByIdAsync(int questionId);
        Task UpdateQuestionAsync(Question question);

        Task<int> AddQuestionAsync(Question question);
        Task AddAnswerQuestionsAsync(List<AnswerQuestion> answers);

    }
}
