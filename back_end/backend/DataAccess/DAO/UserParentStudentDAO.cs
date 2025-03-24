using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.DataAccess.DAO
{
    public class UserParentStudentDAO
    {
        private readonly MyDbContext _dbContext;

        public UserParentStudentDAO(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<User>> GetAllStudentByParentId(int parentId)
        {
            return await _dbContext.UserParentStudents
                .Where(u => u.ParentId == parentId)
                .Include(u => u.Student.Grade)
                .Select(u => u.Student)
                .ToListAsync();
        }
    }
}
