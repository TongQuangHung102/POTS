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
        private readonly IUserSubscriptionService _userSubscriptionService;

        public LoginService(IAuthRepository authRepository, PasswordEncryption passwordEncryption, IGradeRepository gradeRepository, IUserSubscriptionService userSubscriptionService)
        {
            _authRepository = authRepository;
            _passwordEncryption = passwordEncryption;
            _gradeRepository = gradeRepository;
            _userSubscriptionService = userSubscriptionService;
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
            if (user.IsLocked)
            {
                throw new UnauthorizedAccessException("Tài khoản đã bị khoá, Vui lòng liên hệ để được hỗ trợ");
            }
            await _authRepository.UpdateLastLoginTimeAsync(user);

            if(user.Role == 1)
            {
                var (isTrial, expirationDate, maxExercises) = await _userSubscriptionService.GetSubscriptionInfoAsync(user.UserId);
                return new LoginResponseDto
                {
                    UserId = user.UserId,
                    Role = user.Role,
                    GradeId = user.GradeId,
                    Token = GenerateJwtToken(user, isTrial, expirationDate, maxExercises)
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
                    CreateAt = DateTime.UtcNow.AddHours(7),
                    IsActive = true,
                    Role = userGoogleDto.Role,
                    LastLogin = DateTime.UtcNow.AddHours(7)
                };

                await _authRepository.CreateUserAsync(user);
            }

            if(user != null && user.GoogleId == null)
            {
                if (user.IsLocked)
                {
                    throw new UnauthorizedAccessException("Tài khoản đã bị khoá, Vui lòng liên hệ để được hỗ trợ");
                }

                user.GoogleId = userGoogleDto.GoogleId;
                user.LastLogin = DateTime.UtcNow.AddHours(7);

                await _authRepository.UpdateUser(user);
            }

            user.LastLogin = DateTime.UtcNow.AddHours(7);

            await _authRepository.UpdateUser(user);
            if (user.Role == 1)
            {
                var (isTrial, expirationDate, maxExercises) = await _userSubscriptionService.GetSubscriptionInfoAsync(user.UserId);

                return new LoginResponseDto
                {
                    UserId = user.UserId,
                    Role = user.Role,
                    GradeId = user.GradeId,
                    Token = GenerateJwtToken(user, isTrial, expirationDate, maxExercises)
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

        private string GenerateJwtToken(User user, bool? IsTrial = null, DateTime? ExpirationDate = null,int? maxExercises = null)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("UltraSecureKey_ForJWTAuth!987654321@2025$");


            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(ClaimTypes.Email, user.Email)
            };

            if (IsTrial.HasValue)
            {
                claims.Add(new Claim("IsTrial", IsTrial.Value.ToString()));
            }

            if (ExpirationDate.HasValue)
            {
                claims.Add(new Claim("ExpirationDate", ExpirationDate.Value.ToString("yyyy-MM-dd HH:mm:ss")));  
            }
            if (maxExercises.HasValue)
            {
                claims.Add(new Claim("maxExercises", maxExercises.Value.ToString()));
            }

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddDays(1),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

    }

}
