using backend.Dtos.Subject;
using backend.Services;
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

        [HttpPut("edit-subject")]
        public async Task<IActionResult> EditSubject(SubjectEditDto dto)
        {
            await _subjectService.EditSubjectAsync(dto);
            return Ok("Chỉnh sửa môn học thành công.");
        }
    }
}
