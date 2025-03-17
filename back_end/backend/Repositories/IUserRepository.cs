using backend.Models;

namespace backend.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetUsersAsync(int? roleId, string? email, int skip, int take);
        Task<int> GetTotalUsersAsync(int? roleId, string? email, int? gradeId = null);
        Task<User?> GetUserByIdAsync(int userId);
        Task UpdateUserAsync(User user);
        Task<User?> GetUserByEmailAsync(string email);
        Task CreateUserAsync(User user);
        Task<List<User>> GetUsersByRoleAsync(int roleId);
        Task<User> GetAllInfomationUser(int userId);
        Task<int> GetTotalNewStudent(int date, int? gradeId = null);
        Task<int> GetTotalStudentByDate(DateTime date, int? gradeId = null);

    }
}
