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
     
        [HttpGet("get-chapter-by-grade")]
        public async Task<ActionResult<List<ChapterDto>>> GetAllChapter(int gradeId)
        {
            try
            {
                var chapters = await _chapterService.GetAllChaptersAsync(gradeId);

                if (chapters == null || !chapters.Any())
                {
                    return NoContent(); 
                }

                return Ok(chapters);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", error = ex.Message });
            }
        }

        [HttpGet("get-lesson-by-chapterId")]
        public async Task<ActionResult<List<LessonDto>>> GetAllLessonByChapterId(int chapterId)
        {
            try
            {
                var lessons = await _lessonService.GetAllLessonByChapterIdAsync(chapterId);

                if (lessons == null || !lessons.Any())
                {
                    return NoContent();
                }

                return Ok(lessons);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", error = ex.Message });
            }
        }

        [HttpPost("add-chapters")]
        public async Task<IActionResult> AddChapters([FromBody] AddChaptersRequest chaptersRequest)
        {
            try
            {
                await _chapterService.AddChaptersAsync(chaptersRequest.GradeId,chaptersRequest.Semester, chaptersRequest.Input);
                return Ok("Chương được thêm thành công!");
            }
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch(FormatException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception)
            {
                return StatusCode(500, "Lỗi máy chủ nội bộ");
            }
        }
        
        [HttpPut("edit-chapter/{id}")]
        public async Task<IActionResult> EditChapter(int id, [FromBody] ChapterDto chapterDto)
        {
            try
            {
                await _chapterService.EditChapterAsync(id, chapterDto);
                return Ok("Chỉnh sửa chương thành công");
            }
            catch (KeyNotFoundException)
            {
                return NotFound("Chương không tồn tại");
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

        [HttpPost("add-lessons")]
        public async Task<IActionResult> AddLessons([FromBody] AddLessonsRequest request)
        {
            try
            {
                await _lessonService.AddLessonsFromStringAsync(request.ChapterId, request.Input);
                return Ok("Bài được thêm mới thành công");
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


        [HttpPut("edit-lesson/{id}")]
        public async Task<IActionResult> EditLesson(int id, [FromBody] LessonDto lessonDto)
        {
            try
            {
                await _lessonService.EditLessonAsync(id, lessonDto);
                return Ok("Cập nhật bài thành công.");
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
