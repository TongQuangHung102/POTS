using backend.Models;

namespace backend.Repositories
{
    public interface IPracticeQuestionRepository
    {
        Task<PracticeQuestion> CreateAsync(PracticeQuestion practiceQuestion);
        void DeletePracticeQuestions(List<PracticeQuestion> practiceQuestions);
        Task AddAnswerQuestionsAsync(List<AnswerPracticeQuestion> answers);
        Task<List<PracticeQuestion>> GetExpiredUnansweredQuestionsAsync(DateTime expired);
        Task SaveAsync();
    }

}
