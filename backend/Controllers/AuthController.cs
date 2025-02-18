using backend.DataAccess.DAO;
using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using static System.Net.WebRequestMethods;
using backend.Models;

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

            var result = await _registerService.Register(model);

            return result;
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
                return Ok("Password reset successfully. Check your email.");
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("login-google")]
        public IActionResult LoginWithGoogle()
        {
            var properties = new AuthenticationProperties
            {
                RedirectUri = Url.Action("GoogleCallback", "Auth", null, Request.Scheme)
            };

            return Challenge(properties, GoogleDefaults.AuthenticationScheme);
        }

        [HttpGet("google-callback")]
        public async Task<IActionResult> GoogleCallback()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(GoogleDefaults.AuthenticationScheme);

            if (!authenticateResult.Succeeded)
                return Unauthorized();

            var claims = authenticateResult.Principal.Identities.First().Claims.ToList();

            var googleUserDto = new UserGoogleDto
            {
                GoogleId = claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value,
                Email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value,
                UserName = claims.FirstOrDefault(c => c.Type == "name")?.Value
               ?? claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName)?.Value
               ?? "Unknown",
                CreateAt = DateTime.UtcNow,
                IsActive = true,
                Role = 1
            };

            var response = await _loginService.FindOrCreateUserGoogleAsync(googleUserDto);

            return Ok(response);
        }

    }
}
