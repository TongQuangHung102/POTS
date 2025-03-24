using backend.Dtos.Curriculum;
using backend.Models;
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
        private readonly SubjectGradeService _subjectGradeService;

        public CurriculumController(ChapterService chapterService, LessonService lessonService, SubjectGradeService subjectGradeService)
        {
            _chapterService = chapterService;
            _lessonService = lessonService;
            _subjectGradeService = subjectGradeService;
        }

        [HttpGet("get-chapter-by-grade/{gradeId}/subject/{subjectId}")]
        public async Task<IActionResult> GetAllChapter(int gradeId, int subjectId)
        {
            try
            {
                var subjectGrade = await _subjectGradeService.GetBySubjectGradeAsync(gradeId, subjectId);
                if (subjectGrade == null)
                {
                    return NoContent(); 
                }

                return Ok(new
                {
                    subjectGradeId = subjectGrade.Id,
                    gradeName = subjectGrade.Grade.GradeName,
                    subjectName = subjectGrade.Subject.SubjectName,
                    chapters = subjectGrade.Chapters.Select(c => new
                    {
                        chapterId = c.ChapterId,
                        chapterName = c.ChapterName,
                        order = c.Order,
                        semester = c.Semester,
                        isVisible = c.IsVisible
                    })
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Lỗi máy chủ nội bộ", error = ex.Message });
            }
        }

        [HttpGet("get-chapter-and-num-question-by-grade/{gradeId}/subject/{subjectId}")]
        public async Task<IActionResult> GetAllChapterAndNumQuestion(int gradeId, int subjectId)
        {
            try
            {
                var chapters = await _chapterService.GetChaptersWithQuestionLevelsAsync(gradeId, subjectId);
                if (chapters == null || chapters.Count == 0)
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

        [HttpGet("get-student-chapter/{gradeId}/subject/{subjectId}/student/{studentId}")]
        public async Task<ActionResult<List<ChapterDto>>> GetStudentChapter(int gradeId,int subjectId, int studentId)
        {
            var subjectGrade = await _subjectGradeService.GetBySubjectGradeAsync(gradeId, subjectId);
            try
            {
                var chapters = await _chapterService.GetFilteredChaptersAsync(subjectGrade.Id, studentId);

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
        public async Task<IActionResult> GetAllLessonByChapterId(int chapterId)
        {
            try
            {
                var chapter = await _chapterService.GetChapterByIdAsync(chapterId);

                if (chapter == null)
                {
                    return NoContent();
                }

                return Ok(new
                {
                    chapterId = chapter.ChapterId,
                    chapterName = "Chương " + chapter.Order + ": " + chapter.ChapterName,
                    lessons = chapter.Lessons.Select(c => new
                    {
                        lessonId = c.LessonId,
                        lessonName = c.LessonName,
                        order = c.Order,
                        isVisible = c.IsVisible
                    })
                });
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
                await _chapterService.AddChaptersAsync(chaptersRequest.subjectgradeId, chaptersRequest.Semester, chaptersRequest.Input);
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
            catch (ArgumentException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (FormatException ex)
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
