using backend.Models;
using System.Threading.Tasks;

namespace backend.Repositories
{
    public interface IReportRepository 
    {
        Task<List<Report>> GetAllReportsAsync(int pageNumber, int pageSize, int subjectGradeId, string? status = null);
        Task<Report> GetReportByIdAsync(int reportId);
        Task AddReportAsync(Report report);
        Task<int> GetTotalReportsAsync(int subjectGradeId, string? status = null);
        Task UpdateReport(Report report);
        Task<Report> GetReportByQuestionAndReason(int questionId, int reason);
        Task<int> GetTotalReportCount(int subjectGradeId);
        Task<int> GetTotalReportCountByStatus(string status, int subjectGradeId);
        Task<List<Report>> GetTop5PendingReports(int subjectGradeId);
        Task<(List<string> Labels, List<int> Data)> GetReportStatisticsByReason(int subjectGradeId);
        Task<List<Report>> GetReportsValidByDateRange(DateTime fromDate, int subjectGradeId);

    }
}
