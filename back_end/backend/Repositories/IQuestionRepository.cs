using backend.Dtos.Dashboard;
using backend.Models;

namespace backend.Repositories
{
    public interface IQuestionRepository
    {
        Task<List<Question>> GetAllQuestionsAsync(int? chapterId, int? lessonId, int? levelId, string searchTerm, bool? isVisible, int page, int pageSize);
        Task<int> GetTotalQuestionsAsync(int? chapterId, int? lessonId, int? levelId, string searchTerm, bool? isVisible);
        Task<Question?> GetQuestionByIdAsync(int questionId);
        Task UpdateQuestionAsync(Question question);
        Task<List<Question>> GetQuestionsFirstTimePractice(int count, int lessonId);
        Task<int> AddQuestionAsync(Question question);
        Task AddAnswerQuestionsAsync(List<AnswerQuestion> answers);
        Task<List<Question>> GetQuestionsPractice(int count, int lessonId, int levelId);
        Task<int> CountQuestionInGrade(int gradeId);
        Task<List<Question>> GetQuestionsByChapterAutoAsync(ChapterQuestionAutoRequest chapterRequest);

    }
}
