using backend.DataAccess.DAO;
using backend.Dtos.Questions;
using backend.Dtos.Report;
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

        public async Task<List<Report>> GetAllReportsAsync(int pageNumber, int pageSize, string? status = null)
        {
            return await _reportRepository.GetAllReportsAsync(pageNumber, pageSize, status);
        }

        public async Task<int> GetTotalReportsAsync(string? status = null)
        {
            return await _reportRepository.GetTotalReportsAsync(status);
        }

        public async Task<Report> GetReportByIdAsync(int reportId)
        {
            return await _reportRepository.GetReportByIdAsync(reportId);
        }

        public async Task AddReportAsync(ReportDto dto)
        {
            var report = new Report
            {
                Reason = dto.Reason,
                QuestionId = dto.QuestionId,
                UserId = dto.UserId,
                CreatedAt = DateTime.UtcNow,
                Status = "Chờ kiểm tra"
            };

            await _reportRepository.AddReportAsync(report);
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
    }
}
