using backend.Dtos.PracticeAndTest;
using backend.Dtos.SubjectGrade;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubjectGradeController : ControllerBase
    {
        private readonly SubjectGradeService _subjectGradeService;

        public SubjectGradeController(SubjectGradeService subjectGradeService)
        {
            _subjectGradeService = subjectGradeService;
        }
        [Authorize(Roles = "1,2,3,4")]
        [HttpGet("get-subject-by-grade/{id}")]
        public async Task<IActionResult> GetSubjectByGrade(int id)
        {
            var subjects = await _subjectGradeService.GetAllSubjectByGradeAsync(id);
            var results = subjects.Select(x => new
            {
                id = x.Id,
                name =  x.Subject.SubjectName,
                grade = x.Grade.GradeName,
                gradeId = x.GradeId,
                subjectId = x.SubjectId
            });
            return Ok(results);
        }
        [Authorize(Roles = "4")]
        [HttpPost("add-subject-grade")]
        [Consumes("multipart/form-data")]
        public async Task<IActionResult> AddSubjectGrade(
            [FromForm] int SubjectId,  
            [FromForm] int GradeId,
            [FromForm] SubjectGradeFiles files)
        {
            try
            {
                var dto = new SubjectGradeDto
                {
                    SubjectId = SubjectId,
                    GradeId = GradeId
                };

                await _subjectGradeService.AddNewSubjectGrade(dto, files.FileKy1, files.FileKy2);

                return Ok(new { message = "Thêm thành công!" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    message = "Đã có lỗi xảy ra, vui lòng thử lại sau.",
                    details = ex.ToString() 
                });
            }
        }

        public class SubjectGradeFiles
        {
            public IFormFile FileKy1 { get; set; }
            public IFormFile FileKy2 { get; set; }
        }

    }
}
