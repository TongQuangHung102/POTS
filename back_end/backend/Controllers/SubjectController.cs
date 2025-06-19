using backend.Dtos.Subject;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class SubjectController : ControllerBase
    {
        private readonly SubjectService _subjectService;

        public SubjectController(SubjectService subjectService)
        {
            _subjectService = subjectService;
        }
        [Authorize]
        [HttpGet("get-all-subject")]
        public async Task<IActionResult> GetAllSubjects()
        {
            var subjects = await _subjectService.GetAllSubjects();
            var reponse = subjects.Select(s => new
            {
                id = s.SubjectId,
                name = s.SubjectName,
                isVisible = s.IsVisible
            });
            return Ok(reponse);
        }
        [Authorize(Roles = "3")]
        [HttpPost("add-subject")]
        public async Task<IActionResult> AddNewSubject([FromBody] SubjectDto dto)
        {
            try
            {
                await _subjectService.AddNewSubjectAsync(dto);
                return Ok(new { message = "Thêm môn học thành công" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Đã xảy ra lỗi hệ thống", error = ex.Message });
            }
        }
        [Authorize(Roles = "3")]
        [HttpPut("edit-subject")]
        public async Task<IActionResult> EditSubject([FromBody] SubjectEditDto dto)
        {
            try
            {
                await _subjectService.EditSubjectAsync(dto);
                return Ok(new { Message = "Chỉnh sửa môn học thành công" });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Message = "Đã xảy ra lỗi trong quá trình chỉnh sửa môn học.", Details = ex.Message });
            }
        }
    }
}
