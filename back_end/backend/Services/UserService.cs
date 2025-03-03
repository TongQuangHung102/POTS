using backend.DataAccess.DAO;
using backend.Dtos;
using backend.Helpers;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace backend.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordEncryption _passwordEncryption;
        public UserService(IUserRepository userRepository, PasswordEncryption passwordEncryption)
        {
            _userRepository = userRepository;
            _passwordEncryption = passwordEncryption;
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

        public async Task<IActionResult> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return new NotFoundObjectResult(new { Message = "User not found" });
                }

                var response = new
                {
                    user.UserId,
                    user.UserName,
                    user.Email,
                    user.CreateAt,
                    user.LastLogin,
                    user.IsActive,
                    RoleId = user.Role,
                    RoleName = user.RoleNavigation?.RoleName,
                    user.GradeId
                };

                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = "An error occurred while retrieving the user.", error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }

        public async Task<IActionResult> UpdateUserAsync(int userId, UserDto userDto)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return new NotFoundObjectResult(new { Message = "User not found" });
                }


                user.UserName = userDto.UserName;
                user.Email = userDto.Email;
                user.Role = userDto.Role;
                user.IsActive = userDto.IsActive;

      
                if (!string.IsNullOrEmpty(userDto.Password))
                {
                    user.Password = _passwordEncryption.HashPassword(userDto.Password);
                }

                await _userRepository.UpdateUserAsync(user);

                return new OkObjectResult(new { Message = "User updated successfully" });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = "An error occurred while updating the user.", error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }

        public async Task<IActionResult> CreateUserAsync(UserDto userDto)
        {
            try
            {

                var existingUser = await _userRepository.GetUserByEmailAsync(userDto.Email);
                if (existingUser != null)
                {
                    return new BadRequestObjectResult(new { Message = "Email already exists" });
                }


                var newUser = new User
                {
                    UserName = userDto.UserName,
                    Email = userDto.Email,
                    Role = userDto.Role,
                    IsActive = userDto.IsActive,
                    CreateAt = DateTime.UtcNow,
                    LastLogin = null,
                    Password = _passwordEncryption.HashPassword(userDto.Password)
                };

                await _userRepository.CreateUserAsync(newUser);

                return new OkObjectResult(new { Message = "User created successfully" });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = "An error occurred while creating the user.", error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }

        public async Task<IActionResult> UpdateRoleUserAsync(int userId, int roleId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return new NotFoundObjectResult(new { Message = "Người dùng không tồn tại" });
                }

                if (user.Role != null && user.Role > 0)
                {
                    return new BadRequestObjectResult(new { Message = "Bạn đã chọn vai trò!" });
                }

                user.Role = roleId;

                await _userRepository.UpdateUserAsync(user);

                return new OkObjectResult(new { Message = "Cập nhật vai trò thành công" });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = "Có lỗi xảy ra.", error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }

        public async Task<IActionResult> UpdateGradeUserAsync(int userId, int gradeId)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return new NotFoundObjectResult(new { Message = "Người dùng không tồn tại" });
                }

                if (user.GradeId != null && user.GradeId > 0)
                {
                    return new BadRequestObjectResult(new { Message = "Bạn đã chọn lớp!" });
                }

                user.GradeId = gradeId;

                await _userRepository.UpdateUserAsync(user);

                return new OkObjectResult(new { Message = "Cập nhật khối lớp thành công" });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = "Có lỗi xảy ra.", error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }
        public async Task<IActionResult> GetUsersByRoleAsync(int roleId)
        {
            try
            {
                var users = await _userRepository.GetUsersByRoleAsync(roleId);
                if (users == null || users.Count == 0)
                {
                    return new NotFoundObjectResult(new { Message = "Không tìm thấy người dùng nào với vai trò này." });
                }

                var response = users.Select(u => new
                {
                    u.UserId,
                    u.UserName,
                    u.Email,
                    u.CreateAt,
                    u.LastLogin,
                    u.IsActive,
                    RoleId = u.Role,
                    RoleName = u.RoleNavigation?.RoleName
                });

                return new OkObjectResult(response);
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { message = "Có lỗi xảy ra khi lấy danh sách người dùng.", error = ex.Message })
                {
                    StatusCode = 500
                };
            }
        }

    }
}
