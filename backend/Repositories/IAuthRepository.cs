using backend.Models;

namespace backend.Repositories
{
    public interface IAuthRepository
    {
        Task AddUser(User user);
        Task UpdateUser(User user);
        Task<User> GetUserByToken(string token);
        Task<User> GetUserByEmail(string email);
        Task UpdatePasswordAsync(string email, string newPassword);
        Task UpdateLastLoginTimeAsync(User user);



    }
}
