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

        public async Task<int> GetTotalUsersAsync(int? roleId, string email, int? gradeId = null)
        {
            return await _userDAO.GetTotalUsersAsync(roleId, email, gradeId);
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
        public async Task<List<User>> GetUsersByRoleAsync(int roleId)
        {
            return await _userDAO.GetUsersByRoleAsync(roleId);
        }

        public async Task<User> GetAllInfomationUser(int userId)
        {
            return await _userDAO.GetAllInfomationUser(userId);
        }

        public async Task<int> GetTotalNewStudent(int date, int? gradeId = null)
        {
           return await _userDAO.GetTotalNewStudent(date, gradeId);
        }

        public async Task<int> GetTotalStudentByDate(DateTime date , int? gradeId = null)
        {
            return await _userDAO.GetTotalStudentByDate(date, gradeId);
        }
    }
}
