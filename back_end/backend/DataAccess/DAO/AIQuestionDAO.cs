using backend.Models;

namespace backend.DataAccess.DAO
{
    public class AIQuestionDAO
    {
        private readonly MyDbContext _context;

        public AIQuestionDAO(MyDbContext context)
        {
            _context = context;
        }

        public async Task AddAIQuestionAsync(AIQuestion question)
        {
            _context.AIQuestions.Add(question);
            await _context.SaveChangesAsync(); 
            await _context.Entry(question).ReloadAsync();
        }
        public async Task AddAIAnswerQuestionsAsync(AnswerQuestion answer)
        {
            _context.AnswerQuestions.Add(answer);
            await _context.SaveChangesAsync();
        }
    }
}
