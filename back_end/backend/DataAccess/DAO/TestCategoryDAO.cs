using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.DataAccess.DAO
{
    public class TestCategoryDAO
    {
        private readonly MyDbContext _context;

        public TestCategoryDAO(MyDbContext context)
        {
            _context = context;
        }

        public async Task<List<TestCategory>> GetAllAsync()
        {
            return await _context.TestCategories.ToListAsync();
        }
        public async Task<TestCategory> GetByIdAsync(int id)
        {
            return await _context.TestCategories.FindAsync(id);
        }

        public async Task AddTestCategoryAsync(TestCategory category)
        {
            _context.TestCategories.AddAsync(category);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateTestCategoryAsync(TestCategory category)
        {
            _context.TestCategories.Update(category);
            await _context.SaveChangesAsync();
        }
    }
}
