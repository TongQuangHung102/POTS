using backend.DataAccess.DAO;
using backend.Models;

namespace backend.Repositories
{
    public class ReportRepository : IReportRepository
    {
        private readonly ReportDAO _reportDAO;

        public ReportRepository(ReportDAO reportDAO)
        {
            _reportDAO = reportDAO;
        }

        public async Task AddReportAsync(Report report)
        {
            await _reportDAO.AddReportAsync(report);
        }

        public async Task<List<Report>> GetAllReportsAsync(int pageNumber, int pageSize, int subjectGradeId, string? status = null)
        {
           return await _reportDAO.GetAllReportsAsync(pageNumber, pageSize,subjectGradeId, status);
        }

        public async Task<Report> GetReportByIdAsync(int reportId)
        {
            return await _reportDAO.GetReportByIdAsync(reportId);
        }

        public async Task<Report> GetReportByQuestionAndReason(int questionId, int reason)
        {
           return await _reportDAO.GetReportByQuestionAndReason(questionId, reason);
        }

        public async Task<(List<string> Labels, List<int> Data)> GetReportStatisticsByReason(int subjectGradeId)
        {
            return await _reportDAO.GetReportStatisticsByReason(subjectGradeId);
        }

        public async Task<List<Report>> GetReportsValidByDateRange(DateTime fromDate, int subjectGradeId)
        {
            return await _reportDAO.GetReportsValidByDateRange(fromDate, subjectGradeId);
        }

        public async Task<List<Report>> GetTop5PendingReports(int subjectGradeId)
        {
            return await _reportDAO.GetTop5PendingReports(subjectGradeId);
        }

        public async Task<int> GetTotalReportCount(int subjectGradeId)
        {
            return await _reportDAO.GetTotalReportCount(subjectGradeId);
        }

        public async Task<int> GetTotalReportCountByStatus(string status, int subjectGradeId)
        {
            return await _reportDAO.GetTotalReportCountByStatus(status, subjectGradeId);
        }

        public async Task<int> GetTotalReportsAsync(int subjectGradeId, string? status = null)
        {
            return await _reportDAO.GetTotalReportsAsync(subjectGradeId, status);
        }

        public Task UpdateReport(Report report)
        {
            return _reportDAO.UpdateReport(report);
        }

    }
}
