using backend.Models;

namespace backend.Repositories
{
    public interface ISubjectGradeRepository
    {
        Task<List<SubjectGrade>> GetAllSubjectByGradeAsync(int gradeId);
        Task AddAsync(SubjectGrade subjectGrade);
        Task UpdateAsync(SubjectGrade subjectGrade);
        Task<SubjectGrade?> GetByGradeAndSubjectAsync(int gradeId, int subjectId);
        Task<SubjectGrade?> GetGradeSubjectByIdAsync(int id);
        Task<SubjectGrade?> GetTestByGradeAndSubjectAsync(int gradeId, int subjectId);
    }
}
