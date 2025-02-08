using backend.Models;

namespace backend.Repositories
{
    public interface IAuthRepository
    {
        Task AddUser(User user);
    }
}
