using backend.Models;

namespace backend.DataAccess.DAO
{
    public class PracticeQuestionDAO
    {
        private readonly MyDbContext _context;

        public PracticeQuestionDAO(MyDbContext context)
        {
            _context = context;
        }

        public async Task<PracticeQuestion> CreateAsync(PracticeQuestion practiceQuestion)
        {
            _context.PracticeQuestions.Add(practiceQuestion);
            await _context.SaveChangesAsync();
            return practiceQuestion;
        }

        public async Task<bool> DeleteAsync(int questionId)
        {
            var practiceQuestion = await _context.PracticeQuestions.FindAsync(questionId);
            if (practiceQuestion == null)
            {
                return false;
            }

            _context.PracticeQuestions.Remove(practiceQuestion);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task AddAnswerQuestionsAsync(List<AnswerPracticeQuestion> answers)
        {
            await _context.AnswerPracticeQuestions.AddRangeAsync(answers);
            await _context.SaveChangesAsync();
        }
    }
}
