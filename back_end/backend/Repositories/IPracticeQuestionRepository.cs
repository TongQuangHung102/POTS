using backend.Models;

namespace backend.Repositories
{
    public interface IPracticeQuestionRepository
    {
        Task<PracticeQuestion> CreateAsync(PracticeQuestion practiceQuestion);
        Task<bool> DeleteAsync(int questionId);
        Task AddAnswerQuestionsAsync(List<AnswerPracticeQuestion> answers);
    }
}
