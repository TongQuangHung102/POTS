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

        public async Task<List<User>> GetAllStudentByParentId(int parentId)
        {
            return await _userParentStudentDAO.GetAllStudentByParentId(parentId);   
        }
    }
}
