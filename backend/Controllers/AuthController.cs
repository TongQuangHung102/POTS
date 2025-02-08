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
        public AuthController(RegisterService registerService, LoginService loginService)
        {
            _registerService = registerService;
            _loginService = loginService;
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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
        {
            var response = await _loginService.LoginAsync(request);
            if (response == null)
            {
                return Unauthorized(new { Message = "Invalid username or password" });
            }

            return Ok(response);
        }
    }
}
