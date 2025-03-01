using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.DataAccess.DAO
{
    public class UserDAO
    {
        private readonly MyDbContext _context;

        public UserDAO(MyDbContext context)
        {
            _context = context;
        }

        public async Task<List<User>> GetUsersAsync(int? roleId, string email, int skip, int take)
        {
            var query = _context.Users.Include(u => u.RoleNavigation).AsQueryable();

   
            if (roleId.HasValue)
            {
                query = query.Where(u => u.Role == roleId.Value);
            }

     
            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(u => u.Email.Contains(email));
            }

    
            query = query.OrderBy(u => u.UserId); 

            return await query.Skip(skip).Take(take).ToListAsync();
        }

        public async Task<int> GetTotalUsersAsync(int? roleId, string email)
        {
            var query = _context.Users.AsQueryable();

            if (roleId.HasValue)
            {
                query = query.Where(u => u.Role == roleId.Value);
            }

            if (!string.IsNullOrEmpty(email))
            {
                query = query.Where(u => u.Email.Contains(email));
            }

            return await query.CountAsync();
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.RoleNavigation)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetUserByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task CreateUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }
        public async Task<List<User>> GetUsersByRoleAsync(int roleId)
        {
            return await _context.Users
                .Include(u => u.RoleNavigation)
                .Where(u => u.Role == roleId)
                .ToListAsync();
        }
    }
}
