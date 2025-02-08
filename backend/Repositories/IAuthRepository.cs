using backend.Models;

namespace backend.Repositories
{
    public interface IAuthRepository
    {
        Task AddUser(User user);
        Task<User> GetUserByEmail(string email);
        Task UpdatePasswordAsync(string email, string newPassword);
  
      
    }
}
