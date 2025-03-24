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
                gradeName = s.Grade.GradeName
            });

            return Ok(data);
        }
    }
}
