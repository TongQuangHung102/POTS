using backend.DataAccess.DAO;
using backend.Models;

namespace backend.Repositories
{
    public class SubjectGradeRepository : ISubjectGradeRepository
    {
        private readonly SubjectGradeDAO _subjectGradeDAO;

        public SubjectGradeRepository(SubjectGradeDAO subjectGradeDAO)
        {
            _subjectGradeDAO = subjectGradeDAO;
        }

        public async Task AddAsync(SubjectGrade subjectGrade)
        {
            await _subjectGradeDAO.AddAsync(subjectGrade);
        }

        public async Task<List<SubjectGrade>> GetAllSubjectByGradeAsync(int gradeId)
        {
            return await _subjectGradeDAO.GetAllSubjectByGradeAsync(gradeId);
        }

        public async Task<SubjectGrade?> GetByGradeAndSubjectAsync(int gradeId, int subjectId)
        {
            return await _subjectGradeDAO.GetByGradeAndSubjectAsync(gradeId, subjectId);
        }

        public async Task<SubjectGrade?> GetGradeSubjectByIdAsync(int id)
        {
            return await _subjectGradeDAO.GetGradeSubjectByIdAsync(id);
        }

        public async Task<SubjectGrade?> GetTestByGradeAndSubjectAsync(int gradeId, int subjectId)
        {
           return await _subjectGradeDAO.GetTestByGradeAndSubjectAsync(gradeId, subjectId);
        }

        public async Task UpdateAsync(SubjectGrade subjectGrade)
        {
           await _subjectGradeDAO.UpdateAsync(subjectGrade);
        }

    }
}
