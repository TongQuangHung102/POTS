using backend.Dtos;
using backend.Helpers;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace backend.Services
{
    public class RegisterService
    {
        private readonly IAuthRepository _authRepository;

        public RegisterService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }

        public async Task<IActionResult> Register(RegisterDto model)
        {

            string hashedPassword = PasswordEncryption.HashPassword(model.Password);

            var user = new User
            {
                UserName = model.UserName,
                Email = model.Email,
                Password = hashedPassword,
                FullName = model.FullName,
                Role = model.Role,
                IsActive = model.IsActive,
                CreateAt = DateTime.UtcNow
            };


            await _authRepository.AddUser(user);

            return new OkObjectResult(new { message = "User registered successfully!" });
        }
    }
}
