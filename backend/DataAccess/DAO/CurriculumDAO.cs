using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.DataAccess.DAO
{
    public class CurriculumDAO
    {
        private readonly MyDbContext _context;

        public CurriculumDAO(MyDbContext context)
        {
            _context = context;
        }

        public async Task AddChaptersAsync(List<Chapter> chapters)
        {
            await _context.Chapters.AddRangeAsync(chapters);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Chapter>> GetAllChapterAsync()
        {
            return await _context.Chapters.Include(m => m.User).ToListAsync();
        }
    }
}
