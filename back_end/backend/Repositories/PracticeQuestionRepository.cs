using backend.DataAccess.DAO;
using backend.Models;

namespace backend.Repositories
{
    public class PracticeQuestionRepository : IPracticeQuestionRepository
    {
        private readonly PracticeQuestionDAO _practiceQuestionDAO;

        public PracticeQuestionRepository(PracticeQuestionDAO practiceQuestionDAO)
        {
            _practiceQuestionDAO = practiceQuestionDAO;
        }

        public async Task AddAnswerQuestionsAsync(List<AnswerPracticeQuestion> answers)
        {
            await _practiceQuestionDAO.AddAnswerQuestionsAsync(answers);    
        }

        public async Task<PracticeQuestion> CreateAsync(PracticeQuestion practiceQuestion)
        {
            return await _practiceQuestionDAO.CreateAsync(practiceQuestion);
        }

        public void DeletePracticeQuestions(List<PracticeQuestion> practiceQuestions)
        {
            _practiceQuestionDAO.DeletePracticeQuestions(practiceQuestions);
        }

        public async Task<List<PracticeQuestion>> GetExpiredUnansweredQuestionsAsync(DateTime expired)
        {
            return await _practiceQuestionDAO.GetExpiredUnansweredQuestionsAsync(expired);
        }

        public async Task SaveAsync()
        {
            await _practiceQuestionDAO.SaveAsync();
        }
    }
}
