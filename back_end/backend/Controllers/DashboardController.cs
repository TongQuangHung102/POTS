using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly StudentService _studentService;
        private readonly AdminService _adminService;
        private readonly ContentManageService _contentManageService;

        public DashboardController(StudentService studentService, AdminService adminService, ContentManageService contentManageService)
        {
            _studentService = studentService;
            _adminService = adminService;
            _contentManageService = contentManageService;
        }

        [HttpGet("student-dashboard/user/{userId}/subjectGrade/{subjectGradeId}")]
        public async Task<IActionResult> GetDashboardData(int userId, int subjectGradeId)
        {
            var dashboardData = await _studentService.GetDashboardDataAsync(userId, subjectGradeId);
            return Ok(dashboardData);
        }

        [HttpGet("admin-dashboard")]
        public async Task<IActionResult> GetAdminDashboardData([FromQuery] int? gradeId)
        {
            var dashboardData = await _adminService.GetAdminDashboardData(gradeId);
            return Ok(dashboardData);
        }

        [HttpGet("content-manage-dashboard/{gradeId}/{subjectId}")]
        public async Task<IActionResult> GetContentManageDashboardData(int gradeId, int subjectId)
        {
            var dashboardData = await _contentManageService.GetContentManageDashboardData(gradeId, subjectId);
            return Ok(dashboardData);
        }
    }
}
