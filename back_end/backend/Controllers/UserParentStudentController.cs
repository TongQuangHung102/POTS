using backend.Dtos.Users;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserParentStudentController : ControllerBase
    {
        private readonly UserParentStudentService _userParentStudentService;

        public UserParentStudentController(UserParentStudentService userParentStudentService)
        {
            _userParentStudentService = userParentStudentService;
        }

        [HttpGet("get-students/{parentId}")]
        public async Task<IActionResult> GetStudentsByParent(int parentId)
        {
            var students = await _userParentStudentService.GetAllStudentByParentId(parentId);
            if (students == null)
                return NotFound("Không có học sinh nào được tìm thấy.");

            var data = students.Select(s => new
            {
                userId = s.UserId,
                userName = s.UserName,
                lastLogin = s.LastLogin,
                gradeId = s.GradeId,
                gradeName = s.Grade.GradeName,
                email = s.Email
            });

            return Ok(data);
        }

        [HttpPost("link-account")]
        public async Task<IActionResult> LinkAccountChild([FromBody] LinkAccount linkAccount)
        {
            try
            {
                await _userParentStudentService.LinkAccountChild(linkAccount);
                return Ok(new { message = "Mã xác nhận đã được gửi đến email của học sinh." });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra.", error = ex.Message });
            }
        }

        [HttpPost("verify-link-account")]
        public async Task<IActionResult> VerifyLinkAccount([FromBody] VerifyLinkAccount verifyDto)
        {
            try
            {
                bool isVerified = await _userParentStudentService.VerifyLinkAccountAsync(verifyDto.ParentId, verifyDto.StudentEmail, verifyDto.Code);
                return Ok(new { message = "Liên kết tài khoản thành công." });
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Có lỗi xảy ra.", error = ex.Message });
            }
        }

        [HttpPost("resend-verification-code")]
        public async Task<IActionResult> ResendVerificationCode([FromBody] LinkAccount request)
        {
            var result = await _userParentStudentService.ResendVerificationCodeAsync(request.Email, request.ParentId);

            if (result.Contains("Không tìm thấy") || result.Contains("không tồn tại"))
            {
                return NotFound(new { message = result });
            }

            return Ok(new { message = result });
        }

        [HttpDelete("unlink")]
        public async Task<IActionResult> DeleteParentStudent([FromQuery] int parentId, [FromQuery] int studentId)
        {
            try
            {
                await _userParentStudentService.DeleteParentStudentAsync(parentId, studentId);
                return Ok(new { message = "Xóa liên kết thành công" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "Xóa liên kết thất bại", error = ex.Message });
            }
        }
    }
}
