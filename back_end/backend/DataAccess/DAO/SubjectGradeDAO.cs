using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.DataAccess.DAO
{
    public class SubjectGradeDAO
    {
        private readonly MyDbContext _dbContext;

        public SubjectGradeDAO(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<SubjectGrade>> GetAllSubjectByGradeAsync(int gradeId)
        {
            return await _dbContext.SubjectGrades
                .Where(g => g.GradeId == gradeId && g.Subject.IsVisible == true)
                .Include(sg => sg.Subject)
                .Include(sg => sg.Grade)
                .ToListAsync();
        }

        public async Task AddAsync(SubjectGrade subjectGrade)
        {
            _dbContext.SubjectGrades.Add(subjectGrade);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(SubjectGrade subjectGrade)
        {
            _dbContext.SubjectGrades.Update(subjectGrade);
            await _dbContext.SaveChangesAsync();
        }
        public async Task<SubjectGrade?> GetByGradeAndSubjectAsync(int gradeId, int subjectId)
        {
            return await _dbContext.SubjectGrades
                .Include(sg => sg.Chapters)
                .Include(sg => sg.Subject)
                .Include(sg => sg.Grade)
                .FirstOrDefaultAsync(sg => sg.GradeId == gradeId && sg.SubjectId == subjectId);
        }
        public async Task<SubjectGrade?> GetGradeSubjectByIdAsync(int id)
        {
            return await _dbContext.SubjectGrades
                .Include(sg => sg.Chapters)
                .Include(sg => sg.Subject)
                .Include(sg => sg.Grade)
                .FirstOrDefaultAsync(s => s.Id == id);
        }
        public async Task<SubjectGrade?> GetTestByGradeAndSubjectAsync(int gradeId, int subjectId)
        {
            return await _dbContext.SubjectGrades
                .Include(sg => sg.Tests)
                .Include(sg => sg.Subject)
                .Include(sg => sg.Grade)
                .FirstOrDefaultAsync(sg => sg.GradeId == gradeId && sg.SubjectId == subjectId);
        }


    }
}
