using backend.Models;

namespace backend.Repositories
{
    public interface ISubjectRepository
    {
        Task<List<Subject>> GetAllSubjectAsync();
        Task<Subject?> GetSubjectByIdAsync(int subjectId);
        Task AddSubjectAsync(Subject subject);
        Task UpdateSubjectAsync(Subject subject);
        Task<Subject> GetSubjectByName(string name);
    }
}
