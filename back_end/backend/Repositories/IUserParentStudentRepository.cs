using backend.Models;

namespace backend.Repositories
{
    public interface IUserParentStudentRepository
    {
        Task<List<User>> GetAllStudentByParentId(int parentId);
    }
}
