using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class UserParentStudentService
    {
        private readonly IUserParentStudentRepository _userParentStudentRepository;

        public UserParentStudentService(IUserParentStudentRepository userParentStudentRepository)
        {
            _userParentStudentRepository = userParentStudentRepository;
        }

        public async Task<List<User>> GetAllStudentByParentId(int parentId)
        {
            return await _userParentStudentRepository.GetAllStudentByParentId(parentId);    
        }
    }
}
