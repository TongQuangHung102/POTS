using backend.Services;
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

        [HttpGet("List")]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int? roleId = null,
            [FromQuery] string? email = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return await _userService.GetUsersAsync(roleId, email, page, pageSize);
        }

    }
}
