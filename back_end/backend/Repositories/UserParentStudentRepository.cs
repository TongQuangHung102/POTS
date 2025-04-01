using backend.DataAccess.DAO;
using backend.Models;

namespace backend.Repositories
{
    public class UserParentStudentRepository : IUserParentStudentRepository
    {
        private readonly UserParentStudentDAO _userParentStudentDAO;

        public UserParentStudentRepository(UserParentStudentDAO userParentStudentDAO)
        {
            _userParentStudentDAO = userParentStudentDAO;
        }

        public async Task CreateParentStudentAsync(UserParentStudent request)
        {
             await _userParentStudentDAO.CreateParentStudentAsync(request);
        }

        public async Task DeleteParentStudentAsync(int parentId, int studentId)
        {
            await _userParentStudentDAO.DeleteParentStudentAsync(parentId, studentId);
        }

        public async Task<List<User>> GetAllStudentByParentId(int parentId)
        {
            return await _userParentStudentDAO.GetAllStudentByParentId(parentId);   
        }

        public async Task<UserParentStudent> GetByParentIdAndStudentId(int parentId, int studentId)
        {
            return await _userParentStudentDAO.GetByParentIdAndStudentId(parentId, studentId);
        }

        public async Task<User> GetParentByStudentIdAsync(int studentId)
        {
            return await _userParentStudentDAO.GetParentByStudentIdAsync(studentId);
        }

        public async Task UpdateParentStudentAsync(UserParentStudent request)
        {
            await _userParentStudentDAO.UpdateParentStudentAsync(request);
        }
    }
}
