using backend.DataAccess.DAO;
using backend.Dtos.Questions;
using backend.Dtos.Report;
using backend.Helpers;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace backend.Services
{
    public class ReportService
    {
        private readonly IReportRepository _reportRepository;
        private readonly IQuestionRepository _questionRepository;

        public ReportService(IReportRepository reportRepository, IQuestionRepository questionRepository)
        {
            _reportRepository = reportRepository;
            _questionRepository = questionRepository;
        }

        public async Task<List<Report>> GetAllReportsAsync(int pageNumber, int pageSize,int subjectGradeId, string? status = null)
        {
            return await _reportRepository.GetAllReportsAsync(pageNumber, pageSize, subjectGradeId, status);
        }

        public async Task<int> GetTotalReportsAsync(int subjectGradeId, string? status = null)
        {
            return await _reportRepository.GetTotalReportsAsync(subjectGradeId, status);
        }

        public async Task<Report> GetReportByIdAsync(int reportId)
        {
            return await _reportRepository.GetReportByIdAsync(reportId);
        }

        public async Task AddReportAsync(ReportDto dto)
        {
            try
            {
                var reasonEnum = (ReportReason)dto.Reason;

                var existingReport = await _reportRepository.GetReportByQuestionAndReason(dto.QuestionId, (int)reasonEnum);

                if (existingReport != null)
                {
                    if (existingReport.UserId == dto.UserId)
                    {
                        throw new InvalidOperationException("Bạn đã báo cáo câu hỏi này!");
                    }
                    else
                    {
                        existingReport.ReportCount += 1;
                        await _reportRepository.UpdateReport(existingReport);
                        return;
                    }
                }

                var report = new Report
                {
                    Reason = reasonEnum,
                    QuestionId = dto.QuestionId,
                    UserId = dto.UserId,
                    CreatedAt = DateTime.UtcNow,
                    Status = "Pending",
                    ReportCount = 1
                };

                await _reportRepository.AddReportAsync(report);
            }
            catch (InvalidOperationException ex)
            {
                throw new Exception(ex.Message);
            }
            catch (Exception ex)
            {
                throw new Exception("Đã xảy ra lỗi trong quá trình xử lý yêu cầu. Vui lòng thử lại sau.");
            }
        }


        public async Task UpdateReport(ReportEditDto dto)
        {
            var report = await _reportRepository.GetReportByIdAsync(dto.ReportId);

            if (report == null) {
                throw new KeyNotFoundException("Báo cáo không tồn tại.");
            }

            if(dto.Status == "Resolved")
            {
                var question = await _questionRepository.GetQuestionByIdAsync(dto.QuestionId);

                if (question == null)
                {
                    throw new Exception("Câu hỏi không tồn tại.");
                }
                question.QuestionText = dto.Question.QuestionText;
                question.CorrectAnswer = dto.Question.CorrectAnswer;

                foreach (var answerDto in dto.Question.AnswerQuestions)
                {
                    var existingAnswer = question.AnswerQuestions
                        .FirstOrDefault(a => a.AnswerQuestionId == answerDto.AnswerQuestionId);

                    if (existingAnswer != null)
                    {
                        existingAnswer.AnswerText = answerDto.AnswerText;
                    }
                    else
                    {
                        throw new InvalidOperationException("Không thể thêm câu trả lời mới khi cập nhật.");
                    }
                }

                await _questionRepository.UpdateQuestionAsync(question);
            }

            report.Status = dto.Status;
            await _reportRepository.UpdateReport(report);

        }

        public async Task<ReportDashboard> GetReportDashboardAsync(int subjectGradeId)
        {
            int totalReport = await _reportRepository.GetTotalReportCount(subjectGradeId);
            int validReport = await _reportRepository.GetTotalReportCountByStatus("Resolved", subjectGradeId);
            int inValidReport = await _reportRepository.GetTotalReportCountByStatus("Reject", subjectGradeId);
            int pendingReport = await _reportRepository.GetTotalReportCountByStatus("Pending", subjectGradeId);

            var(labels, data) = await _reportRepository.GetReportStatisticsByReason(subjectGradeId);
            var listReport = await _reportRepository.GetTop5PendingReports(subjectGradeId);

            var dataDashboard = new ReportDashboard
            {
                TotalReport = totalReport,
                ValidReport = validReport,
                InValidReport = inValidReport,
                PendingReport = pendingReport,
                TotalReportByReason = new TotalReportByReasonDto
                {
                    Data = data,
                    Labels = labels
                },
                ReportInDashboards = listReport.Select(r => new ReportInDashboard
                {
                    Id = r.ReportId,
                    QuestionId = r.QuestionId,
                    Reason = EnumHelper.GetEnumDescription((ReportReason)r.Reason),
                    ReportCount = r.ReportCount,
                    Status = r.Status
                }).ToList(),
            };
            return dataDashboard;
        }

    }
}
