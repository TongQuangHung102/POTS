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
        private readonly IUserSubscriptionService _userSubscriptionService;

        public UserController(UserService userService, IUserSubscriptionService userSubscriptionService)
        {
            _userService = userService;
            _userSubscriptionService = userSubscriptionService;
        }

        [Authorize(Roles = "3")]
        [HttpGet("get-all-user")]
        public async Task<IActionResult> GetUsers(
            [FromQuery] int? roleId = null,
            [FromQuery] string? email = null,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var (totalUsers, users) = await _userService.GetUsersAsync(roleId, email, page, pageSize);

                if (!string.IsNullOrEmpty(email) && users.Count == 0)
                {
                    return Ok(new
                    {
                        Message = "Không tìm thấy người dùng phù hợp.",
                        TotalUsers = totalUsers,
                        Page = 1,
                        PageSize = pageSize,
                        Data = new List<object>()
                    });
                }

                var response = new
                {
                    TotalUsers = totalUsers,
                    Page = page,
                    PageSize = pageSize,
                    Data = users.Select(u => new
                    {
                        u.UserId,
                        u.UserName,
                        u.Email,
                        u.CreateAt,
                        u.LastLogin,
                        u.IsActive,
                        u.IsLocked,
                        RoleId = u.Role,
                        u.RoleNavigation?.RoleName
                    })
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Có lỗi xảy ra khi lấy danh sách người dùng.",
                    error = ex.Message
                });
            }
        }
        [Authorize]
        [HttpGet("get-user-by/{userId}")]
        public async Task<IActionResult> GetUserByIdAsync(int userId)
        {
            try
            {
                var user = await _userService.GetUserByIdAsync(userId);
                if (user == null)
                {
                    return NotFound(new { Message = "Không tìm thấy người dùng." });
                }

                var response = new
                {
                    user.UserId,
                    user.UserName,
                    user.Email,
                    user.CreateAt,
                    user.LastLogin,
                    user.IsActive,
                    user.IsLocked,
                    RoleId = user.Role,
                    RoleName = user.RoleNavigation?.RoleName,
                    user.GradeId
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Đã xảy ra lỗi khi lấy thông tin người dùng.",
                    error = ex.Message
                });
            }
        }
        [Authorize]
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
        [Authorize]
        [HttpPut("edit-user/{userId}")]
        public async Task<IActionResult> UpdateUser(int userId, [FromBody] UserEditDto userDto)
        {
            try
            {
                var success = await _userService.UpdateUserAsync(userId, userDto);
                if (!success)
                {
                    return NotFound(new { Message = "Không tìm thấy người dùng." });
                }

                return Ok(new { Message = "Cập nhật người dùng thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Đã xảy ra lỗi khi cập nhật người dùng.",
                    error = ex.Message
                });
            }
        }
        [Authorize]
        [HttpPost("add-user")]
        public async Task<IActionResult> CreateUser([FromBody] UserDto userDto)
        {
            try
            {
                var (success, message) = await _userService.CreateUserAsync(userDto);
                if (!success)
                {
                    return BadRequest(new { Message = message });
                }

                return Ok(new { Message = "Tạo người dùng thành công." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Đã xảy ra lỗi khi tạo người dùng.",
                    error = ex.Message
                });
            }
        }
        [Authorize]
        [HttpPut("update-role/{userId}")]
        public async Task<IActionResult> UpdateRoleUser(int userId, [FromBody] int roleId)
        {
            try
            {
                var (success, message) = await _userService.UpdateUserRoleAsync(userId, roleId);
                if (!success)
                {
                    return BadRequest(new { Message = message });
                }

                return Ok(new { Message = "Cập nhật vai trò thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Đã xảy ra lỗi khi cập nhật vai trò.",
                    error = ex.Message
                });
            }
        }
        [Authorize]
        [HttpPut("update-grade/{userId}")]
        public async Task<IActionResult> UpdateGradeUser(int userId, [FromBody] int gradeId)
        {
            try
            {
                var (success, message) = await _userService.UpdateUserGradeAsync(userId, gradeId);
                if (!success)
                {
                    return BadRequest(new { Message = message });
                }

                return Ok(new { Message = "Cập nhật khối lớp thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Đã xảy ra lỗi khi cập nhật khối lớp.",
                    error = ex.Message
                });
            }
        }
        [Authorize]
        [HttpGet("get-user-by-roleId/{roleId}")]
        public async Task<IActionResult> GetUsersByRole(int roleId)
        {
            try
            {
                var users = await _userService.GetUsersByRoleAsync(roleId);

                if (users == null || !users.Any())
                {
                    return NotFound(new { Message = "Không tìm thấy người dùng nào với vai trò này." });
                }

                var response = users.Select(u => new
                {
                    u.UserId,
                    u.UserName,
                    u.Email,
                    u.CreateAt,
                    u.LastLogin,
                    u.IsActive,
                    RoleId = u.Role,
                    RoleName = u.RoleNavigation?.RoleName
                });

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Đã xảy ra lỗi khi lấy danh sách người dùng.",
                    error = ex.Message
                });
            }
        }
        [Authorize]
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
        [Authorize(Roles = "2")]
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
        [Authorize(Roles = "1")]
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

        [Authorize(Roles = "1,2")]
        [HttpGet("get-plan-user/{userId}")]
        public async Task<IActionResult> GetPlanUser(int userId)
        {
            try
            {
                var subscription = await _userSubscriptionService.GetByUserIdAsync(userId);

                if (subscription == null)
                {
                    return NotFound(new { message = "Không tìm thấy gói đăng ký của người dùng." });
                }

                var response = new
                {
                    planName = subscription.SubscriptionPlan.PlanName,
                    endDate = subscription.EndDate,
                    startDate = subscription.StartDate,
                    price = subscription.SubscriptionPlan.Price
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra khi lấy gói đăng ký của người dùng.", error = ex.Message });
            }
        }

    }
}
