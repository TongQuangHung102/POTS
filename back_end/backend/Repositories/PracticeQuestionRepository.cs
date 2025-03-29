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

        public async Task<bool> DeleteAsync(int questionId)
        {
            return await _practiceQuestionDAO.DeleteAsync(questionId);
        }
    }
}
