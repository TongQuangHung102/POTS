using backend.Dtos.Auth;
using backend.Helpers;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace backend.Services
{
    public class RegisterService
    {
        private readonly IAuthRepository _authRepository;
        private readonly ISendMailService _sendMailService;
        private readonly PasswordEncryption _passwordEncryption;
        private readonly IUserSubscriptionService _userSubscriptionService;
        private readonly string _frontendUrl = "http://localhost:3000/login";

        public RegisterService(IAuthRepository authRepository, ISendMailService sendMailService, PasswordEncryption passwordEncryption, IUserSubscriptionService userSubscriptionService)
        {
            _authRepository = authRepository;
            _sendMailService = sendMailService;
            _passwordEncryption = passwordEncryption;
            _userSubscriptionService = userSubscriptionService;
        }

        public async Task RegisterAsync(RegisterDto model)
        {
            var existingUser = await _authRepository.GetUserByEmail(model.Email);
            if (existingUser != null)
            {
                throw new Exception("Email đã tồn tại, vui lòng dùng email khác");
            }

            var hashedPassword = _passwordEncryption.HashPassword(model.Password);
            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                Password = hashedPassword,
                Role = model.Role,
                IsActive = false,
                IsLocked = false,
                CreateAt = DateTime.UtcNow.AddHours(7),
                EmailVerificationToken = Guid.NewGuid().ToString(),
                TokenExpiry = DateTime.UtcNow.AddHours(7).AddHours(24)
            };

            await _authRepository.AddUser(user);
            await _sendMailService.SendConfirmationEmailAsync(user.Email, user.EmailVerificationToken);
        }

        public async Task ResendConfirmationEmailAsync(string email)
        {
            var user = await _authRepository.GetUserByEmail(email);
            if (user == null)
            {
                throw new Exception("Email không tồn tại trong hệ thống");
            }

            if (user.IsActive)
            {
                throw new Exception("Tài khoản đã được xác thực trước đó");
            }

            user.EmailVerificationToken = Guid.NewGuid().ToString();
            user.TokenExpiry = DateTime.UtcNow.AddHours(7).AddHours(24);

            await _authRepository.UpdateUser(user);

            try
            {
                await _sendMailService.SendConfirmationEmailAsync(user.Email, user.EmailVerificationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi gửi lại email xác nhận: {ex.Message}");
            }
        }

        public async Task<string> ConfirmEmailAsync(string token)
        {
            var user = await _authRepository.GetUserByToken(token);
            if (user == null)
            {
                return $"{_frontendUrl}?status=invalid_token";
            }

            if (user.TokenExpiry < DateTime.UtcNow)
            {
                return $"{_frontendUrl}?status=token_expired";
            }

            user.IsActive = true;
            user.EmailVerificationToken = null;
            user.TokenExpiry = null;
            await _authRepository.UpdateUser(user);
            await _userSubscriptionService.AssignFreePlanToUserAsync(user.UserId);

            return $"{_frontendUrl}?status=success";
        }

    }
}
