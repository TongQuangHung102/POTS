using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.DataAccess.DAO
{
    public class ReportDAO
    {
        private readonly MyDbContext _dbContext;

        public ReportDAO(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<Report>> GetAllReportsAsync(int pageNumber, int pageSize, string? status = null)
        {
            var query = _dbContext.Reports
                .Include(r => r.User)
                .Include(r => r.Question)
                    .ThenInclude(q => q.AnswerQuestions)
                .AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(r => r.Status == status);
            }

            var reports = await query
                .OrderByDescending(r => r.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return reports;
        }

        public async Task<int> GetTotalReportsAsync(string? status = null)
        {
            var query = _dbContext.Reports.AsQueryable();

            if (!string.IsNullOrEmpty(status))
            {
                query = query.Where(r => r.Status == status);
            }

            return await query.CountAsync();
        }



        public async Task<Report> GetReportByIdAsync(int reportId)
        {
            return await _dbContext.Reports.FindAsync(reportId);
        }

        public async Task AddReportAsync(Report report)
        {
            await _dbContext.Reports.AddAsync(report);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateReport(Report report)
        {
             _dbContext.Reports.Update(report);
            await _dbContext.SaveChangesAsync();
        }
    }
}
