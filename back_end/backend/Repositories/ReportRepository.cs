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

        public async Task<List<Report>> GetAllReportsAsync(int pageNumber, int pageSize, string? status = null)
        {
           return await _reportDAO.GetAllReportsAsync(pageNumber, pageSize, status);
        }

        public async Task<Report> GetReportByIdAsync(int reportId)
        {
            return await _reportDAO.GetReportByIdAsync(reportId);
        }

        public async Task<int> GetTotalReportsAsync(string? status = null)
        {
            return await _reportDAO.GetTotalReportsAsync(status);
        }

        public Task UpdateReport(Report report)
        {
            return _reportDAO.UpdateReport(report);
        }
    }
}
