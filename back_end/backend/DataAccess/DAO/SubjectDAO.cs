using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.DataAccess.DAO
{
    public class SubjectDAO
    {
        private readonly MyDbContext _dbContext;

        public SubjectDAO(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        public async Task<List<Subject>> GetAllSubjectAsync()
        {
            return await _dbContext.Subjects.ToListAsync();
        }

        public async Task<Subject?> GetSubjectByIdAsync(int subjectId)
        {
            return await _dbContext.Subjects.FindAsync(subjectId);
        }

        public async Task AddSubjectAsync(Subject subject)
        {
            await _dbContext.Subjects.AddAsync(subject);
            await _dbContext.SaveChangesAsync();
        }
        public async Task UpdateSubjectAsync(Subject subject)
        {
            _dbContext.Subjects.Update(subject);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<Subject> GetSubjectByName(string name)
        {
            return await _dbContext.Subjects.Where(s => s.SubjectName  == name).FirstOrDefaultAsync();
        }
    }
}
