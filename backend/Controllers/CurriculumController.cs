using backend.Dtos;
using backend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CurriculumController : ControllerBase
    {
        private readonly ChapterService _chapterService;

        public CurriculumController(ChapterService chapterService)
        {
            _chapterService = chapterService;
        }
        [Authorize]
        [HttpGet("get-all-chapter")]
        public async Task<ActionResult<List<ChapterDto>>> GetAllPlans()
        {
            var chapters = await _chapterService.GetAllChaptersAsync();
            return Ok(chapters);
        }
        [Authorize]
        [HttpPost("add-chapters")]
        public async Task<IActionResult> AddChapters([FromBody] string input)
        {
            try
            {
                await _chapterService.AddChaptersAsync(input);
                return Ok("Chapters added successfully");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
    }
}
