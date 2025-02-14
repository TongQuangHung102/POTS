using backend.DataAccess.DAO;
using backend.Models;

namespace backend.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly UserDAO _userDAO;

        public UserRepository(UserDAO userDAO)
        {
            _userDAO = userDAO;
        }

        public async Task<List<User>> GetUsersAsync(int? roleId, string email, int skip, int take)
        {
            return await _userDAO.GetUsersAsync(roleId, email, skip, take);
        }

        public async Task<int> GetTotalUsersAsync(int? roleId, string email)
        {
            return await _userDAO.GetTotalUsersAsync(roleId, email);
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _userDAO.GetUserByIdAsync(userId);
        }

        public async Task UpdateUserAsync(User user)
        {
            await _userDAO.UpdateUserAsync(user);
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _userDAO.GetUserByEmailAsync(email);
        }

        public async Task CreateUserAsync(User user)
        {
            await _userDAO.CreateUserAsync(user);
        }
    }
}
