using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly UserService _userService;

        public UserController(UserService userService)
        {
            _userService = userService;
        }
        [Authorize]
        [HttpGet("List")]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int? roleId = null,
            [FromQuery] string? email = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return await _userService.GetUsersAsync(roleId, email, page, pageSize);
        }
        [Authorize]
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            return await _userService.GetUserByIdAsync(userId);
        }
        [Authorize]
        [HttpPut("{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UserDto userDto)
        {
            return await _userService.UpdateUserAsync(userId, userDto);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
        {
            return await _userService.CreateUserAsync(userDto);
        }
    }
}
