using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace backend.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }


        public async Task<IActionResult> GetUsersAsync(int? roleId, string? email, int page, int pageSize)
        {
            try
            {
                email ??= string.Empty;                 
                var totalUsers = await _userRepository.GetTotalUsersAsync(roleId, email);                
                if (!string.IsNullOrEmpty(email))
                {
                    page = 1;
                }
                int skip = (page - 1) * pageSize;
                var users = await _userRepository.GetUsersAsync(roleId, email, skip, pageSize);
                if (!string.IsNullOrEmpty(email) && users.Count == 0)
                {
                    return new OkObjectResult(new
                    {
                        Message = "Không tìm thấy người dùng phù hợp.",
                        TotalUsers = totalUsers,
                        Page = page,
                        PageSize = pageSize,
                        Data = users
                    });
                }


                var response = new
                {
                    TotalUsers = totalUsers,
                    Page = page,
                    PageSize = pageSize,
                    Data = users.Select(u => new
                    {
                        u.UserId,
                        u.UserName,
                        u.Email,
                        u.FullName,
                        u.CreateAt,
                        u.LastLogin,
                        u.IsActive,
                        RoleId = u.Role,
                        u.RoleNavigation?.RoleName
                    })
                };

                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = "An error occurred while retrieving users.", error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }



    }
}
