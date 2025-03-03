using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class LevelController : ControllerBase
    {
        private readonly LevelService _levelService;

        public LevelController(LevelService levelService)
        {
            _levelService = levelService;
        }

        [HttpGet("get-all-level")]
        public async Task<IActionResult> GetAllLevel()
        {
            return await _levelService.GetAllLevel();
        }
    }
}
