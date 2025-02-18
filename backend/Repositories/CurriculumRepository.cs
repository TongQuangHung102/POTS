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

        public async Task<List<Chapter>> GetAllChapterAsync()
        {
            return await _curriculumDAO.GetAllChapterAsync();
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
    }
}
