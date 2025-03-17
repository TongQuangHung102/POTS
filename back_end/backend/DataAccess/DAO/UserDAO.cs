using backend.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

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

        public async Task<int> GetTotalUsersAsync(int? roleId, string email, int? gradeId = null)
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
            if (gradeId.HasValue)
            {
                query = query.Where(u => u.GradeId == gradeId.Value);
            }

            return await query.CountAsync();
        }

        public async Task<int> GetTotalNewStudent(int date, int? gradeId = null)
        {
            var query = _context.Users
               .Where(u => u.Role == 1 && u.CreateAt >= DateTime.Today.AddDays(-date))
                .AsQueryable();

            if (gradeId.HasValue) 
            {
                query = query.Where(u => u.GradeId == gradeId.Value);
            }

            return await query.CountAsync();
        }

        public async Task<int> GetTotalStudentByDate(DateTime date, int? gradeId = null)
        {
            var query = _context.Users
                .Where(u => u.Role == 1 && u.CreateAt.Date == date.Date)
                .AsQueryable();

            if (gradeId.HasValue) 
            {
                query = query.Where(u => u.GradeId == gradeId.Value);
            }

            return await query.CountAsync();
        }

        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _context.Users
                .Include(u => u.RoleNavigation)
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User> GetAllInfomationUser(int userId)
        {
            return await _context.Users
                .Include(u => u.RoleNavigation)
                .Include(m => m.Grade)
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
