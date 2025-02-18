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
        private readonly LessonService _lessonService;

        public CurriculumController(ChapterService chapterService, LessonService lessonService)
        {
            _chapterService = chapterService;
            _lessonService = lessonService;
        }
     
        [HttpGet("get-all-chapter")]
        public async Task<ActionResult<List<ChapterDto>>> GetAllChapter()
        {
            var chapters = await _chapterService.GetAllChaptersAsync();
            return Ok(chapters);
        }

        [HttpGet("get-lesson-by-chapterId")]
        public async Task<ActionResult<List<LessonDto>>> GetAllLessonByChapterId(int chapterId)
        {
            var lessons = await _lessonService.GetAllLessonByChapterIdAsync(chapterId);
            return Ok(lessons);
        }

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
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request");
            }
        }
        [HttpPut("edit-chapter/{id}")]
        public async Task<IActionResult> EditChapter(int id, [FromBody] ChapterDto chapterDto)
        {
            try
            {
                await _chapterService.EditChapterAsync(id, chapterDto);
                return Ok("Chapter updated successfully");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Chapter not found");
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPost("add-lessons")]
        public async Task<IActionResult> AddLessons(int chapterId, [FromBody] string input)
        {
            try
            {
                await _lessonService.AddLessonsFromStringAsync(chapterId, input);
                return Ok("Lessons added successfully");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "An error occurred while processing your request");
            }
        }

        [HttpPut("edit-lesson/{id}")]
        public async Task<IActionResult> EditLesson(int id, [FromBody] LessonDto lessonDto)
        {
            try
            {
                await _lessonService.EditLessonAsync(id, lessonDto);
                return Ok("Lesson updated successfully");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Lesson not found");
            }
            catch (InvalidOperationException ex)
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
