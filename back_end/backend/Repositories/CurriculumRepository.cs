using backend.DataAccess.DAO;
using backend.Models;

namespace backend.Repositories
{
    public class CurriculumRepository : ICurriculumRepository
    {
        private readonly CurriculumDAO _curriculumDAO;

        public CurriculumRepository(CurriculumDAO curriculumDAO)
        {
            _curriculumDAO = curriculumDAO;
        }

        public async Task AddChaptersAsync(List<Chapter> chapters)
        {
            await _curriculumDAO.AddChaptersAsync(chapters);
        }

        public async Task AddLessonsAsync(List<Lesson> lessons)
        {
            await _curriculumDAO.AddLessonsAsync(lessons);
        }

        public async Task<List<Chapter>> GetAllChapterAsync(int subjectgradeId)
        {
            return await _curriculumDAO.GetAllChapterAsync(subjectgradeId);
        }

        public async Task<List<Lesson>> GetAllLessonAsync()
        {
            return await _curriculumDAO.GetAllLessonAsync();
        }

        public async Task<Chapter> GetChapterByIdAsync(int id)
        {
            return await _curriculumDAO.GetChapterByIdAsync(id);
        }

        public async Task<List<Lesson>> GetLessonByChapterIdAsync(int id)
        {
            return await _curriculumDAO.GetLessonByChapterIdAsync(id);
        }

        public async Task<Lesson> GetLessonByIdAsync(int id)
        {
            return await _curriculumDAO.GetLessonByIdAsync(id);
        }

        public async Task UpdateChapterAsync(Chapter chapter)
        {
            await _curriculumDAO.UpdateChapterAsync(chapter);
        }

        public async Task UpdateLessonAsync(Lesson lesson)
        {
            await _curriculumDAO.UpdateLessonAsync(lesson);
        }
        public async Task<List<Chapter>> GetChaptersByIdsAsync(List<int> chapterIds)
        {
            return await _curriculumDAO.GetChaptersByIdsAsync(chapterIds);
        }

        public async Task UpdateChaptersAsync(List<Chapter> chapters)
        {
            await _curriculumDAO.UpdateChaptersAsync(chapters);
        }

        public async Task<Lesson> GetLessonWithQuestionsByIdAsync(int id)
        {
            return await _curriculumDAO.GetLessonWithQuestionsByIdAsync(id);
        }

        public async Task<Lesson> GetLessonWithQuestionsAIByIdAsync(int id)
        {
            return await _curriculumDAO.GetLessonWithQuestionsAIByIdAsync(id);
        }

        public async Task<List<Chapter>> GetChaptersWithQuestionsBySubjectGradeAsync(int subjectGradeId)
        {
            return await _curriculumDAO.GetChaptersWithQuestionsBySubjectGradeAsync(subjectGradeId);
        }
    }
}
