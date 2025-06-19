using backend.Models;

namespace backend.Repositories
{
    public interface IUserParentStudentRepository
    {
        Task CreateParentStudentAsync(UserParentStudent request);
        Task<List<User>> GetAllStudentByParentId(int parentId);
        Task UpdateParentStudentAsync(UserParentStudent request);
        Task<UserParentStudent> GetByParentIdAndStudentId(int parentId, int studentId);
        Task DeleteParentStudentAsync(int parentId, int studentId);
        Task<List<User>> GetParentsByStudentIdAsync(int studentId);
        Task<int> GetParentCountAsync(int? gradeId);
        Task<int> CountStudentsByParent(int parentId);
        Task<int> CountParentsByStudent(int studentId);
    }
}
