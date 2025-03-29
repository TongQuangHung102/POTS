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
                .Where(u => u.ParentId == parentId && u.IsVerified == true)
                .Include(u => u.Student.Grade)
                .Select(u => u.Student)
                .ToListAsync();
        }

        public async Task CreateParentStudentAsync(UserParentStudent request)
        {
            _dbContext.UserParentStudents.Add(request);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateParentStudentAsync(UserParentStudent request)
        {
            _dbContext.UserParentStudents.Update(request);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<UserParentStudent> GetByParentIdAndStudentId(int parentId, int studentId)
        {
            return await _dbContext.UserParentStudents.Where(u => u.ParentId == parentId && u.StudentId == studentId).FirstOrDefaultAsync(); 
        }

        public async Task DeleteParentStudentAsync(int parentId, int studentId)
        {
            var entity = await _dbContext.UserParentStudents
                .Where(u => u.ParentId == parentId && u.StudentId == studentId)
                .FirstOrDefaultAsync();

            if (entity != null)
            {
                _dbContext.UserParentStudents.Remove(entity);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<User> GetParentByStudentIdAsync(int studentId)
        {
            var parent = await _dbContext.UserParentStudents
                .Where(ups => ups.StudentId == studentId)
                .Select(ups => ups.Parent)
                .FirstOrDefaultAsync();

            return parent;
        }

    }
}
