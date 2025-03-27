using backend.Dtos.PracticeAndTest;
using backend.Dtos.SubjectGrade;
using backend.Services;
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
        [HttpPost("add-subject-grade")]
        public async Task<IActionResult> AddSubjectGrade(SubjectGradeDto subjectGradeDto)
        {
            try
            {
                await _subjectGradeService.AddNewSubjectGrade(subjectGradeDto);
                return Ok(new { message = "Thêm thành công!" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}
