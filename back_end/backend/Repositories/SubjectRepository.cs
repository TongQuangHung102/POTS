using backend.DataAccess.DAO;
using backend.Models;

namespace backend.Repositories
{
    public class SubjectRepository : ISubjectRepository
    {
        private readonly SubjectDAO _subjectDAO;

        public SubjectRepository(SubjectDAO subjectDAO)
        {
            _subjectDAO = subjectDAO;
        }

        public async Task AddSubjectAsync(Subject subject)
        {
            await _subjectDAO.AddSubjectAsync(subject);
        }

        public async Task<List<Subject>> GetAllSubjectAsync()
        {
            return await _subjectDAO.GetAllSubjectAsync();
        }

        public async Task<Subject?> GetSubjectByIdAsync(int subjectId)
        {
            return await _subjectDAO.GetSubjectByIdAsync(subjectId);
        }

        public async Task<Subject> GetSubjectByName(string name)
        {
            return await _subjectDAO.GetSubjectByName(name);
        }

        public async Task UpdateSubjectAsync(Subject subject)
        {
            await _subjectDAO.UpdateSubjectAsync(subject);
        }
    }
}
