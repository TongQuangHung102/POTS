using backend.Dtos.Curriculum;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GradeController : ControllerBase
    {
        private readonly GradeService _gradeService;

        public GradeController(GradeService gradeService)
        {
            _gradeService = gradeService;
        }
        [Authorize]
        [HttpGet("get-all-grade")]
        public async Task<IActionResult> GetAllGrades()
        {
            try
            {
                var grades = await _gradeService.GetAllGradesAsync();

                var gradeDto = grades.Select(g => new
                {
                    gradeId = g.GradeId,
                    gradeName = g.GradeName,
                    gradeDescription = g.Description,
                    gradeIsVisible = g.IsVisible,
                    userName = g.User?.UserName ?? "Chưa có",
                    userId = g.User?.UserId ?? 0
                });

                return Ok(gradeDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi.", error = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("get-grade/{id}")]
        public async Task<IActionResult> GetGradeById(int id)
        {
            try
            {
                var grade = await _gradeService.GetGradeByIdAsync(id);
                return Ok(grade);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi.", error = ex.Message });
            }
        }
        [Authorize]
        [HttpGet("get-grade-by-userId/{id}")]
        public async Task<IActionResult> GetGradeByUserId(int id)
        {
            try
            {
                var grade = await _gradeService.GetGradeByUserIdAsync(id);
                return Ok(grade);
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi.", error = ex.Message });
            }
        }
        [Authorize(Roles = "3")]
        [HttpPut("update-grade/{id}")]
        public async Task<IActionResult> UpdateGrade(int id, [FromBody] GradeDto gradeDto)
        {
            try
            {
                await _gradeService.UpdateGradeAsync(id, gradeDto);
                return Ok(new { message = "Cập nhật grade thành công!" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi.", error = ex.Message });
            }
        }
        [Authorize(Roles = "3")]
        [HttpPost("add-grade")]
        public async Task<IActionResult> AddGrade([FromBody] string grade)
        {
            try
            {
                await _gradeService.AddGradeAsync(grade);
                return Ok(new { message = "Khối đã được thêm thành công!" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { message = ex.Message }); 
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi.", error = ex.Message });
            }
        }
        [Authorize(Roles = "3")]
        [HttpPut("assign-content-managers")]
        public async Task<IActionResult> AssignContentManagers([FromBody] GradeAssignment request)
        {
            try
            {
                await _gradeService.AssignContentManagersAsync(request);
                return Ok("Gán khối lớp cho Content Manager quản lý thành công!");
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Lỗi máy chủ nội bộ");
            }
        }


    }
}
