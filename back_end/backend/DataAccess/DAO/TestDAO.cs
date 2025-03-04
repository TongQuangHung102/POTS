using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.DataAccess.DAO
{
    public class TestDAO
    {
        private readonly MyDbContext _dbContext;

        public TestDAO(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Test>> GetAllAsync()
        {
            return await _dbContext.Tests.ToListAsync();
        }

        public async Task<Test> GetByIdAsync(int id)
        {
            return await _dbContext.Tests.FindAsync(id);
        }

        public async Task AddAsync(Test test)
        {
            await _dbContext.Tests.AddAsync(test);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Test test)
        {
            _dbContext.Tests.Update(test);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var test = await _dbContext.Tests.FindAsync(id);
            if (test != null)
            {
                _dbContext.Tests.Remove(test);
                await _dbContext.SaveChangesAsync();
            }
        }
        public async Task<List<Test>> GetTestsByGradeIdAsync(int gradeId)
        {
            return await _dbContext.Tests
                .Where(t => t.GradeId == gradeId)
                .ToListAsync();
        }

    }
}
