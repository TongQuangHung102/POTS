using backend.DataAccess.DAO;
using backend.Dtos.Dashboard;
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

        public async Task<List<Question>> GetQuestionsFirstTimePractice(int count, int lessonId)
        {
            return await _questionDAO.GetQuestionsFirstTimePractice(count, lessonId);
        }

        public async Task<List<Question>> GetQuestionsPractice(int count, int lessonId, int levelId)
        {
            return await _questionDAO.GetQuestionsPractice(count, lessonId, levelId);
        }

        public async Task<int> CountQuestionInGrade(int id)
        {
            return await _questionDAO.CountQuestionInSubjectGrade(id);
        }

        public async Task<List<Question>> GetQuestionsByChapterAutoAsync(ChapterQuestionAutoRequest chapterRequest)
        {
            return await _questionDAO.GetQuestionsByChapterAutoAsync(chapterRequest); 
        }

        public async Task MarkQuestionsAsUsed(List<int> questionIds)
        {
            await _questionDAO.MarkQuestionsAsUsed(questionIds);
        }

        public async Task<int> CountQuestionsUsedByWeek(int subjectGradeId, int weekOffset)
        {
            return await _questionDAO.CountQuestionsUsedByWeek(subjectGradeId, weekOffset);
        }
    }
}
