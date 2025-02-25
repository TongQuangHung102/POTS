using backend.Models;

namespace backend.Repositories
{
    public interface IGradeRepository
    {
        Task<List<Grades>> GetAllGradesAsync();
        Task<Grades?> GetGradeByIdAsync(int id);
        Task UpdateGradeAsync(Grades grade);
        Task AddGradeAsync(Grades grade);


    }
}
