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
        private readonly SendMailService _sendMailService;
        private readonly PasswordEncryption _passwordEncryption;
        private readonly string _frontendUrl = "http://localhost:3000/";
        public RegisterService(IAuthRepository authRepository, SendMailService sendMailService, PasswordEncryption passwordEncryption)
        {
            _authRepository = authRepository;
            _sendMailService = sendMailService;
            _passwordEncryption = passwordEncryption;
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
                CreateAt = DateTime.UtcNow,
                EmailVerificationToken = Guid.NewGuid().ToString(),
                TokenExpiry = DateTime.UtcNow.AddHours(24)
            };

            await _authRepository.AddUser(user);
            await _sendMailService.SendConfirmationEmailAsync(user.Email, user.EmailVerificationToken);
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

            return $"{_frontendUrl}?status=success";
        }

    }
}
