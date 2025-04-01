using backend.Dtos.Users;
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

        [HttpGet("get-all-user")]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int? roleId = null,
            [FromQuery] string? email = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            return await _userService.GetUsersAsync(roleId, email, page, pageSize);
        }

        [HttpGet("get-user-by/{userId}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            return await _userService.GetUserByIdAsync(userId);
        }

        [HttpGet("get-user-profile/{userId}")]
        public async Task<IActionResult> GetUserProfileById(int userId)
        {
            try
            {
                var user = await _userService.GetAllInfoUserById(userId);
                if (user == null)
                {
                    return NotFound(new { Message = "Không tìm thấy user." });
                }
                var data = new
                {
                    userId = user.UserId,
                    userName = user.UserName,
                    email = user.Email,
                    grade = user.Grade?.GradeName,
                    createAt = user.CreateAt,
                    role = user.RoleNavigation.RoleName
                };
                return Ok(data);
            }
            catch(Exception ex)
            {
                return StatusCode(500, new { Message = "Lỗi khi thêm mới.", Error = ex.Message });
            }
        }

        [HttpPut("edit-user/{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UserDto userDto)
        {
            return await _userService.UpdateUserAsync(userId, userDto);
        }

        [HttpPost("add-user")]
        public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
        {
            return await _userService.CreateUserAsync(userDto);
        }

        [HttpPut("update-role/{userId}")]
        public async Task<IActionResult> UpdateRoleUser(int userId, [FromBody] int roleId)
        {
            return await _userService.UpdateRoleUserAsync(userId, roleId);
        }

        [HttpPut("update-grade/{userId}")]
        public async Task<IActionResult> UpdateGradeUser(int userId, [FromBody] int gradeId)
        {
            return await _userService.UpdateGradeUserAsync(userId, gradeId);
        }
        [HttpGet("get-user-by-roleId/{roleId}")]
        public async Task<IActionResult> GetUsersByRole(int roleId)
        {
            return await _userService.GetUsersByRoleAsync(roleId);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            try
            {
                await _userService.ChangePassword(request.UserId, request.OldPassword, request.NewPassword);
                return Ok(new { success = true, message = "Đổi mật khẩu thành công!" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { success = false, message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { success = false, message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Lỗi hệ thống!", error = ex.Message });
            }
        }

        [HttpPost("create-student-account")]
        public async Task<IActionResult> CreateStudentAccount([FromBody] CreateAccountByParent model)
        {
            try
            {
                await _userService.CreateStudentAccountAsync(model);
                return Ok(new { message = "Tạo tài khoản học sinh thành công, vui lòng kiểm tra email để xác nhận tài khoản." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { error = ex.Message });
            }
        }

        [HttpPost("change-grade")]
        public async Task<IActionResult> ChangeGrade([FromBody] ChangeGradeDto changeGrade)
        {
            try
            {
                await _userService.ChangeGradeAsync(changeGrade);
                return Ok(new { message = "Đã thay đổi lớp thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi thay đổi lớp.", error = ex.Message });
            }
        }
    }
}
