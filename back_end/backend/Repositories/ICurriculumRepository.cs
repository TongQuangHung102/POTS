using backend.Models;

namespace backend.Repositories
{
    public interface ICurriculumRepository
    {
        Task AddChaptersAsync(List<Chapter> chapters);
        Task<List<Chapter>> GetAllChapterAsync(int subjectgradeId);
        Task<Chapter> GetChapterByIdAsync(int id);
        Task UpdateChapterAsync(Chapter chapter);
        Task AddLessonsAsync(List<Lesson> lessons);
        Task<List<Lesson>> GetAllLessonAsync();
        Task<Lesson> GetLessonByIdAsync(int id);
        Task<List<Lesson>> GetLessonByChapterIdAsync(int id);
        Task UpdateLessonAsync(Lesson lesson);
        Task<List<Chapter>> GetChaptersByIdsAsync(List<int> chapterIds);
        Task UpdateChaptersAsync(List<Chapter> chapters);

        Task<Lesson> GetLessonWithQuestionsByIdAsync(int id);

        Task<Lesson> GetLessonWithQuestionsAIByIdAsync(int id);

    }
}
