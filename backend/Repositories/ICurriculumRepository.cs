using backend.Models;

namespace backend.Repositories
{
    public interface ICurriculumRepository
    {
        Task AddChaptersAsync(List<Chapter> chapters);
        Task<List<Chapter>> GetAllChapterAsync();
    }
}
