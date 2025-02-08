using backend.Models;

namespace backend.DataAccess.DAO
{
    public class AuthDAO
    {
        private readonly MyDbContext _context;

        public AuthDAO(MyDbContext context)
        {
            _context = context;
        }

        public async Task AddUser(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
    }
}
