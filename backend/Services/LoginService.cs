using backend.Dtos;
using backend.Helpers;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace backend.Services
{
    public class LoginService
    {
        private readonly IAuthRepository _authRepository;

        public LoginService(IAuthRepository authRepository)
        {
            _authRepository = authRepository;
        }
        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _authRepository.GetUserByEmail(request.Email);
            if (user == null) return null;

            if (!PasswordEncryption.VerifyPassword(request.Password, user.Password)) return null;

            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("Tài khoản chưa được kích hoạt");
            }
            await _authRepository.UpdateLastLoginTimeAsync(user);
            return new LoginResponseDto
            {
                Username = user.UserName,
                Role = user.Role,
                Token = GenerateJwtToken(user)
            };
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("UltraSecureKey_ForJWTAuth!987654321@2025$");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                        {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Role, user.Role.ToString())
                        }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

}
