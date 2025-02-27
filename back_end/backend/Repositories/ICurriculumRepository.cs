using backend.Models;

namespace backend.Repositories
{
    public interface ICurriculumRepository
    {
        Task AddChaptersAsync(List<Chapter> chapters);
        Task<List<Chapter>> GetAllChapterAsync(int grade);
        Task<Chapter> GetChapterByIdAsync(int id);
        Task UpdateChapterAsync(Chapter chapter);

        Task AddLessonsAsync(List<Lesson> lessons);
        Task<List<Lesson>> GetAllLessonAsync();
        Task<Lesson> GetLessonByIdAsync(int id);
        Task<List<Lesson>> GetLessonByChapterIdAsync(int id);
        Task UpdateLessonAsync(Lesson lesson);
    }
}
