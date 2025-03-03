using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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

        [HttpGet("get-all-grade")]
        public async Task<IActionResult> GetAllGrades()
        {
            return await _gradeService.GetAllGradesAsync();
        }
        [HttpGet("get-grade/{id}")]
        public async Task<IActionResult> GetGradeById(int id)
        {
            return await _gradeService.GetGradeByIdAsync(id);
        }
        [HttpGet("get-grade-by-userId/{id}")]
        public async Task<IActionResult> GetGradeByUserId(int id)
        {
            return await _gradeService.GetGradeByUserIdAsync(id);
        }
        [HttpPut("update-grade/{id}")]
        public async Task<IActionResult> UpdateGrade(int id, [FromBody] GradeDto gradeDto)
        {
            return await _gradeService.UpdateGradeAsync(id, gradeDto);
        }
        [HttpPost("add-grade")]
        public async Task<IActionResult> AddGrade([FromBody] GradeDto gradeDto)
        {
            return await _gradeService.AddGradeAsync(gradeDto);
        }

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
