using backend.Models;

namespace backend.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetUsersAsync(int? roleId, string? email, int skip, int take);
        Task<int> GetTotalUsersAsync(int? roleId, string? email);
    }
}
