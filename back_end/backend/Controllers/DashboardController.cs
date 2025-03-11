using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController : ControllerBase
    {
        private readonly StudentService _studentService;

        public DashboardController(StudentService studentService)
        {
            this._studentService = studentService;
        }

        [HttpGet("student-dashboard/{userId}")]
        public async Task<IActionResult> GetDashboardData(int userId)
        {
            var dashboardData = await _studentService.GetDashboardDataAsync(userId);
            return Ok(dashboardData);
        }
    }
}
