using backend.DataAccess.DAO;
using backend.Models;

namespace backend.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly RoleDAO _roleDAO;

        public RoleRepository(RoleDAO roleDAO)
        {
            _roleDAO = roleDAO;
        }

        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _roleDAO.GetAllRolesAsync();
        }

        public async Task<Role?> GetRoleByIdAsync(int roleId)
        {
            return await _roleDAO.GetRoleByIdAsync(roleId);
        }
    }
}
