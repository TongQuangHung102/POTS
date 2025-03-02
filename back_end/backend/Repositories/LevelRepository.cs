using backend.DataAccess.DAO;
using backend.Models;

namespace backend.Repositories
{
    public class LevelRepository : ILevelRepository
    {
        private readonly LevelDAO _levelDAO;

        public LevelRepository(LevelDAO levelDAO)
        {
            _levelDAO = levelDAO;
        }

        public Task<List<Level>> getAllLevel()
        {
            return _levelDAO.getAllLevel();
        }
    }
}
