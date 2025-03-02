using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.DataAccess.DAO
{
    public class LevelDAO
    {
        private readonly MyDbContext _dbContext;

        public LevelDAO(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Level>> getAllLevel()
        {
            return await _dbContext.Levels.ToListAsync();
        }
    }
}
