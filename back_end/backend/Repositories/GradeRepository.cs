using backend.DataAccess.DAO;
using backend.Models;

namespace backend.Repositories
{
    public class GradeRepository : IGradeRepository
    {
        private readonly GradeDAO _gradeDAO;

        public GradeRepository(GradeDAO gradeDAO)
        {
            _gradeDAO = gradeDAO;
        }

        public async Task<List<Grades>> GetAllGradesAsync()
        {
            return await _gradeDAO.GetAllGradesAsync();
        }
        public async Task<Grades?> GetGradeByIdAsync(int id)
        {
            return await _gradeDAO.GetGradeByIdAsync(id);
        }
        public async Task UpdateGradeAsync(Grades grade)
        {
            await _gradeDAO.UpdateGradeAsync(grade);
        }
        public async Task AddGradeAsync(Grades grade)
        {
            await _gradeDAO.AddGradeAsync(grade);
        }

        public async Task<List<Grades>> GetGradeByUserIdAsync(int id)
        {
            return await _gradeDAO.GetGradeByUserIdAsync(id);
        }

    }
}
