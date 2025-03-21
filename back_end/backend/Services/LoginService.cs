using backend.Dtos.AIQuestions;
using backend.Dtos.Auth;
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
        private readonly PasswordEncryption _passwordEncryption;
        private readonly IGradeRepository _gradeRepository;

        public LoginService(IAuthRepository authRepository, PasswordEncryption passwordEncryption, IGradeRepository gradeRepository)
        {
            _authRepository = authRepository;
            _passwordEncryption = passwordEncryption;
            _gradeRepository = gradeRepository;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginRequestDto request)
        {
            var user = await _authRepository.GetUserByEmail(request.Email);
            if (user == null) return null;
            if (user.Password == null) return null;

            if (!_passwordEncryption.VerifyPassword(request.Password, user.Password)) return null;

            if (!user.IsActive)
            {
                throw new UnauthorizedAccessException("Tài khoản chưa được kích hoạt");
            }
            await _authRepository.UpdateLastLoginTimeAsync(user);

            if(user.Role == 1)
            {
                return new LoginResponseDto
                {
                    UserId = user.UserId,
                    Role = user.Role,
                    GradeId = user.GradeId,
                    Token = GenerateJwtToken(user)
                };
            }
            if (user.Role == 4)
            {
                var grades = await _gradeRepository.GetGradeByUserIdAsync(user.UserId);
                return new LoginResponseDto
                {
                    UserId = user.UserId,
                    Role = user.Role,
                    Grades = grades.Select(g => new GradesResponseDto
                    {
                        Id = g.GradeId,
                        Name = g.GradeName
                    }).ToList(),
                    Token = GenerateJwtToken(user)
                };
            }
            return new LoginResponseDto
            {
                UserId = user.UserId,
                Role = user.Role,
                Token = GenerateJwtToken(user)
            };
        }

        public async Task<LoginResponseDto> FindOrCreateUserGoogleAsync(UserGoogleDto userGoogleDto)
        {
            var user = await _authRepository.GetUserByEmail(userGoogleDto.Email);

            if (user == null)
            {
                user = new User
                {
                    GoogleId = userGoogleDto.GoogleId,
                    Email = userGoogleDto.Email,
                    UserName = userGoogleDto.UserName,
                    CreateAt = DateTime.UtcNow,
                    IsActive = true,
                    Role = userGoogleDto.Role,
                    LastLogin = DateTime.Now
                };

                await _authRepository.CreateUserAsync(user);
            }

            if(user != null && user.GoogleId == null)
            {
                user.GoogleId = userGoogleDto.GoogleId;
                user.LastLogin = DateTime.UtcNow;

                await _authRepository.UpdateUser(user);
            }

            user.LastLogin = DateTime.UtcNow;

            await _authRepository.UpdateUser(user);
            if (user.Role == 1)
            {
                return new LoginResponseDto
                {
                    UserId = user.UserId,
                    Role = user.Role,
                    GradeId = user.GradeId,
                    Token = GenerateJwtToken(user)
                };
            }
            if(user.Role == 4)
            {
                var grades = await _gradeRepository.GetGradeByUserIdAsync(user.UserId);
                return new LoginResponseDto
                {
                    UserId = user.UserId,
                    Role = user.Role,
                    Grades = grades.Select(g => new GradesResponseDto{
                        Id = g.GradeId,
                        Name = g.GradeName
                    }).ToList(),
                    Token = GenerateJwtToken(user)
                };
            }
            return new LoginResponseDto
            {
                UserId = user.UserId,
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
