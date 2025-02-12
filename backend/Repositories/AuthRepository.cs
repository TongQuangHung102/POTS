using backend.DataAccess.DAO;
using backend.Dtos;
using backend.Models;

namespace backend.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly AuthDAO _authDAO;

        public AuthRepository(AuthDAO authDAO)
        {
            _authDAO = authDAO;
        }

        public async Task AddUser(User user)
        {
            await _authDAO.AddUser(user);
        }

        public async Task UpdateUser(User user)
        {
            await _authDAO.UpdateUserAsync(user);
        }

        public async Task<User> GetUserByToken(string token)
        {
            return await _authDAO.GetUserByTokenAsync(token);
        }

        public Task<User> GetUserByEmail(string email)
        {
             return _authDAO.GetUserByEmailAsync(email);
        }

        public async Task UpdateLastLoginTimeAsync(User user)
        {
            await _authDAO.UpdateLastLoginTimeAsync(user);
        }

        // Phương thức cập nhật mật khẩu
        public async Task UpdatePasswordAsync(string email, string newPassword)
        {
            await _authDAO.UpdatePasswordAsync(email, newPassword);
        }


    }
}
