using backend.DataAccess.DAO;
using backend.Dtos.Auth;
using backend.Dtos.Users;
using backend.Helpers;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.ConstrainedExecution;

namespace backend.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;
        private readonly PasswordEncryption _passwordEncryption;
        private readonly ISendMailService _sendMailService;
        private readonly IUserParentStudentRepository _userParentStudentRepository;
        private readonly IUserSubscriptionService _userSubscriptionService;

        public UserService(IUserRepository userRepository, PasswordEncryption passwordEncryption, ISendMailService sendMailService, IUserParentStudentRepository userParentStudentRepository, IUserSubscriptionService userSubscriptionService)
        {
            _userRepository = userRepository;
            _passwordEncryption = passwordEncryption;
            _sendMailService = sendMailService;
            _userParentStudentRepository = userParentStudentRepository;
            _userSubscriptionService = userSubscriptionService;
        }

        public async Task<(int TotalUsers, List<User> Users)> GetUsersAsync(int? roleId, string? email, int page, int pageSize)
        {
            email ??= string.Empty;

            var totalUsers = await _userRepository.GetTotalUsersAsync(roleId, email);

            if (!string.IsNullOrEmpty(email))
            {
                page = 1;
            }

            int skip = (page - 1) * pageSize;
            var users = await _userRepository.GetUsersAsync(roleId, email, skip, pageSize);

            return (totalUsers, users);
        }


        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetUserByIdAsync(userId);
        }

        public async Task<bool> UpdateUserAsync(int userId, UserEditDto userDto)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return false;
            }

            user.UserName = userDto.UserName;
            user.Email = userDto.Email;
            user.Role = userDto.Role;
            user.IsLocked = userDto.IsLocked;

            if (!string.IsNullOrEmpty(userDto.Password))
            {
                user.Password = _passwordEncryption.HashPassword(userDto.Password);
            }

            await _userRepository.UpdateUserAsync(user);
            return true;
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


        public async Task<(bool Success, string? Message)> CreateUserAsync(UserDto userDto)
        {
            var existingUser = await _userRepository.GetUserByEmailAsync(userDto.Email);
            if (existingUser != null)
            {
                return (false, "Email đã tồn tại");
            }

            var newUser = new User
            {
                UserName = userDto.UserName,
                Email = userDto.Email,
                Role = userDto.Role,
                IsActive = false,
                CreateAt = DateTime.UtcNow.AddHours(7),
                LastLogin = null,
                Password = _passwordEncryption.HashPassword(userDto.Password),
                EmailVerificationToken = Guid.NewGuid().ToString(),
                TokenExpiry = DateTime.UtcNow.AddHours(7).AddHours(24)
            };

            await _sendMailService.SendConfirmationEmailAsync(newUser.Email, newUser.EmailVerificationToken);

            await _userRepository.CreateUserAsync(newUser);
            return (true, null);
        }


        public async Task<(bool Success, string? Message)> UpdateUserRoleAsync(int userId, int roleId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return (false, "Người dùng không tồn tại");
            }

            if (user.Role != null && user.Role > 0)
            {
                return (false, "Bạn đã chọn vai trò!");
            }

            user.Role = roleId;
            await _userRepository.UpdateUserAsync(user);

            if (roleId == 1)
            {
                await _userSubscriptionService.AssignFreePlanToUserAsync(userId);
            }

            return (true, null);
        }


        public async Task<(bool Success, string? Message)> UpdateUserGradeAsync(int userId, int gradeId)
        {
            var user = await _userRepository.GetUserByIdAsync(userId);
            if (user == null)
            {
                return (false, "Người dùng không tồn tại");
            }

            if (user.GradeId != null && user.GradeId > 0)
            {
                return (false, "Bạn đã chọn lớp!");
            }

            user.GradeId = gradeId;
            await _userRepository.UpdateUserAsync(user);

            return (true, null);
        }

        public async Task<IEnumerable<User>> GetUsersByRoleAsync(int roleId)
        {
            return await _userRepository.GetUsersByRoleAsync(roleId);
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
                CreateAt = DateTime.UtcNow.AddHours(7),
                EmailVerificationToken = Guid.NewGuid().ToString(),
                TokenExpiry = DateTime.UtcNow.AddHours(7).AddHours(24),
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

        public async Task ChangeGradeAsync(ChangeGradeDto changeGrade)
        {
            var user = await _userRepository.GetUserByIdAsync(changeGrade.UserId);
            if (user == null)
            {
                throw new Exception("Người dùng không tồn tại");
            }
            var currentDate = DateTime.UtcNow.AddHours(7);
            var canChange = await _userRepository.CanChangeGradeAsync(user.UserId);
            if (!canChange) {
                throw new Exception("Không thể đổi lớp vào thời điểm này. Vui lòng đợi 6 tháng hoặc khoảng thời gian từ tháng 6 đến tháng 8 hàng năm");
            }

            user.GradeId = changeGrade.GradeId;
            user.LastGradeChangeDate = currentDate;

            await _userRepository.UpdateUserAsync(user);
        }
    }
}
