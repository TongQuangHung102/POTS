using backend.DataAccess.DAO;
using backend.Services;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using static System.Net.WebRequestMethods;
using backend.Models;
using Google.Apis.Auth;
using backend.Dtos.Auth;
using backend.Dtos.AIQuestions;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly RegisterService _registerService;
        private readonly LoginService _loginService;
        private readonly PasswordResetService _passwordResetService;
        public AuthController(RegisterService registerService, LoginService loginService, PasswordResetService passwordResetService)
        {
            _registerService = registerService;
            _loginService = loginService;
            _passwordResetService = passwordResetService;
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto model)
        {
            try
            {
                var result = await _registerService.Register(model);
                return result;
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi trong quá trình đăng ký.", error = ex.Message });
            }
        }

        [HttpGet("Confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromQuery] string token)
        {
            var result = await _registerService.ConfirmEmailAsync(token);
            return result;
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            try
            {
                var response = await _loginService.LoginAsync(request);

                if (response == null)
                {
                    return Unauthorized(new { Message = "Tài khoản hoặc mật khẩu không chính xác!" });
                }

                return Ok(response);
            }
            catch (UnauthorizedAccessException ex)
            {
                return StatusCode(403, new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã có lỗi xảy ra", Details = ex.Message });
            }
        }
        [HttpPost("Reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] PasswordResetRequestDto request)
        {
            try
            {
                await _passwordResetService.ResetPasswordAsync(request.Email);
                return Ok("Mật khẩu mới đã được gửi về email của bạn");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("google-login")]
        public async Task<IActionResult> GoogleLogin([FromBody] GoogleAuthRequest request)
        {
            try
            {
                var payload = await GoogleJsonWebSignature.ValidateAsync(request.Token);

                var googleDto = new UserGoogleDto
                {
                    GoogleId = payload.Subject,
                    Email = payload.Email,
                    UserName = payload.Name,
                    Role = null
                };

                var loginResponse = await _loginService.FindOrCreateUserGoogleAsync(googleDto);

                return Ok(loginResponse);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Mã token của Google không hợp lệ", error = ex.Message });
            }
        }

    }
}
