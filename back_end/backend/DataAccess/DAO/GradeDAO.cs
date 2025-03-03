using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.DataAccess.DAO
{
    public class GradeDAO
    {
        private readonly MyDbContext _context;

        public GradeDAO(MyDbContext context)
        {
            _context = context;
        }

        public async Task<List<Grades>> GetAllGradesAsync()
        {
            return await _context.Grades.Include(u => u.User).ToListAsync();
        }
        public async Task<Grades?> GetGradeByIdAsync(int id)
        {
            return await _context.Grades.FirstOrDefaultAsync(g => g.GradeId == id);
        }
        public async Task<List<Grades>> GetGradeByUserIdAsync(int id)
        {
            return await _context.Grades.Where(u => u.UserId == id).ToListAsync();
        }
        public async Task UpdateGradeAsync(Grades grade)
        {
            _context.Grades.Update(grade);
            await _context.SaveChangesAsync();
        }
        public async Task AddGradeAsync(Grades grade)
        {
            _context.Grades.Add(grade);
            await _context.SaveChangesAsync();
        }
    }
}
