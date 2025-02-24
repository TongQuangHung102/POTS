using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace backend.Services
{
    public class RoleService
    {
        private readonly IRoleRepository _roleRepository;

        public RoleService(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task<IActionResult> GetAllRolesAsync()
        {
            var roles = await _roleRepository.GetAllRolesAsync();

            var response = roles.Select(r => new
            {
                r.RoleId,
                r.RoleName
            });

            return new OkObjectResult(response);
        }

        public async Task<IActionResult> GetRoleByIdAsync(int roleId)
        {
            var role = await _roleRepository.GetRoleByIdAsync(roleId);

            if (role == null)
            {
                return new NotFoundObjectResult(new { message = "Role not found." });
            }

            return new OkObjectResult(new
            {
                role.RoleId,
                role.RoleName
            });
        }
    }
}
