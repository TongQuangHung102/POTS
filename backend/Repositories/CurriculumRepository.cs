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

        public async Task<List<Chapter>> GetAllChapterAsync()
        {
            return await _curriculumDAO.GetAllChapterAsync();
        }
    }
}
