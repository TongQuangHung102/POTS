using backend.DataAccess.DAO;
using backend.Dtos.Auth;
using backend.Dtos.Users;
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
        private readonly SendMailService _sendMailService;
        private readonly IUserParentStudentRepository _userParentStudentRepository;

        public UserService(IUserRepository userRepository, PasswordEncryption passwordEncryption, SendMailService sendMailService, IUserParentStudentRepository userParentStudentRepository)
        {
            _userRepository = userRepository;
            _passwordEncryption = passwordEncryption;
            _sendMailService = sendMailService;
            _userParentStudentRepository = userParentStudentRepository;
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

        public async Task ChangePassword(int userId, string oldPassword, string newPassword)
        {
            try
            {
                var user = await _userRepository.GetUserByIdAsync(userId);
                if (user == null)
                    throw new KeyNotFoundException("Người dùng không tồn tại.");

                if (user.GoogleId != null)
                    throw new InvalidOperationException("Tài khoản này đăng nhập bằng Google, không thể đổi mật khẩu.");

                if (!_passwordEncryption.VerifyPassword(oldPassword, user.Password))
                    throw new UnauthorizedAccessException("Mật khẩu cũ không đúng.");

                if (_passwordEncryption.VerifyPassword(newPassword, user.Password))
                    throw new ArgumentException("Mật khẩu mới không được trùng với mật khẩu cũ.");

                user.Password = _passwordEncryption.HashPassword(newPassword);
                await _userRepository.UpdateUserAsync(user);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi đổi mật khẩu: {ex.Message}");
                throw;
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

        public async Task<User> GetAllInfoUserById(int userId)
        {
            var user = await _userRepository.GetAllInfomationUser(userId);
            return user;
        }

        public async Task CreateStudentAccountAsync(CreateAccountByParent model)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(model.Email);
            if (existingUser != null)
            {
                throw new Exception("Email đã tồn tại, vui lòng dùng email khác");
            }

            var hashedPassword = _passwordEncryption.HashPassword(model.Password);
            var user = new User
            {
                UserName = model.Username,
                Email = model.Email,
                Password = hashedPassword,
                Role = 1,
                IsActive = false,
                CreateAt = DateTime.UtcNow,
                EmailVerificationToken = Guid.NewGuid().ToString(),
                TokenExpiry = DateTime.UtcNow.AddHours(24),
                GradeId = model.GradeId
            };

            var userId = await _userRepository.CreateUserAndGetUserIdAsync(user);
            await _sendMailService.SendConfirmationEmailAsync(user.Email, user.EmailVerificationToken);

            var userParent = new UserParentStudent
            {
                ParentId = model.ParentId,
                StudentId = userId,
                ExpiryTime = null,
                VerificationCode = "0",
                IsVerified = true,
            };
            await _userParentStudentRepository.CreateParentStudentAsync(userParent);
        }
    }
}
