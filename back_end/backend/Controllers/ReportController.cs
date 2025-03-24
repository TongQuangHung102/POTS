using backend.Dtos.Report;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReportController : Controller
    {
        private readonly ReportService _reportService;

        public ReportController(ReportService reportService)
        {
            _reportService = reportService;
        }

        [HttpGet("get-all-report")]
        public async Task<IActionResult> GetAllReports([FromQuery] int pageNumber, [FromQuery] int pageSize, [FromQuery] string? status = null)
        {
            var reports = await _reportService.GetAllReportsAsync(pageNumber, pageSize, status);
            var total = await _reportService.GetTotalReportsAsync(status);
            var reportResponse = new {
                data = reports.Select(r => new
                {
                    reportId = r.ReportId,
                    reason = r.Reason,
                    createdAt = r.CreatedAt,
                    correct = r.Question.CorrectAnswer,
                    status = r.Status,
                    questionText = r.Question.QuestionText,
                    questionId = r.QuestionId,
                    correctAnswer = r.Question.CorrectAnswer,
                    answerQuestions = r.Question.AnswerQuestions.Select(a => new
                    {
                        id = a.AnswerQuestionId,
                        text = a.AnswerText,
                        number = a.Number
                    })
                }),
                totalReport = total
            };
   
            return Ok(reportResponse);
        }

        [HttpGet("get-report-by-id/{id}")]
        public async Task<IActionResult> GetReportById(int id)
        {
            var report = await _reportService.GetReportByIdAsync(id);
            if (report == null) return NotFound();
            return Ok(report);
        }

        [HttpPost("add-report")]
        public async Task<IActionResult> AddReport([FromBody] ReportDto report)
        {
            if (report == null) return BadRequest();

            await _reportService.AddReportAsync(report);
            return Ok();
        }

        [HttpPut("update-report")]
        public async Task<IActionResult> UpdateReport([FromBody] ReportEditDto dto)
        {
            try
            {
                await _reportService.UpdateReport(dto);
                return Ok(new { message = "Cập nhật báo cáo thành công!" });
            }
            catch (KeyNotFoundException ex)
            {
                return NotFound(new { error = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { error = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "Đã xảy ra lỗi máy chủ!", details = ex.Message });
            }
        }

    }
}
