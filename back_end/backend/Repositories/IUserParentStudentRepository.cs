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
        Task<int?> GetParentIdByStudentIdAsync(int studentId);
    }
}
