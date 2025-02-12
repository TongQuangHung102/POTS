using backend.DataAccess.DAO;
using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

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

    }
}
