using backend.Services;
using Microsoft.AspNetCore.Authorization;
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

        [Authorize(Roles = "1,2")]
        [HttpGet("student-dashboard/user/{userId}/subjectGrade/{subjectGradeId}")]
        public async Task<IActionResult> GetDashboardData(int userId, int subjectGradeId, [FromQuery] int month, [FromQuery] int year)
        {
            try
            {
                var dashboardData = await _studentService.GetDashboardDataAsync(userId, subjectGradeId, month, year);
                return Ok(dashboardData);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Đã xảy ra lỗi khi lấy dữ liệu bảng điều khiển học sinh. Vui lòng thử lại.");
            }
        }

        [Authorize(Roles = "3")]
        [HttpGet("admin-dashboard")]
        public async Task<IActionResult> GetAdminDashboardData([FromQuery] int? gradeId)
        {
            var dashboardData = await _adminService.GetAdminDashboardData(gradeId);
            return Ok(dashboardData);
        }

        [Authorize(Roles = "4")]
        [HttpGet("content-manage-dashboard/{gradeId}/{subjectId}")]
        public async Task<IActionResult> GetContentManageDashboardData(int gradeId, int subjectId)
        {
            var dashboardData = await _contentManageService.GetContentManageDashboardData(gradeId, subjectId);
            return Ok(dashboardData);
        }
    }
}
