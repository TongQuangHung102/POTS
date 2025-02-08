using backend.DataAccess.DAO;
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
    }
}
