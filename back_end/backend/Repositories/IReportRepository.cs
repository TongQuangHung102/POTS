using backend.Models;
using System.Threading.Tasks;

namespace backend.Repositories
{
    public interface IReportRepository 
    {
        Task<List<Report>> GetAllReportsAsync(int pageNumber, int pageSize, string? status = null);
        Task<Report> GetReportByIdAsync(int reportId);
        Task AddReportAsync(Report report);
        Task<int> GetTotalReportsAsync(string? status = null);
        Task UpdateReport(Report report);
    }
}
