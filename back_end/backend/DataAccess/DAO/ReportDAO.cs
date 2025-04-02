using backend.Helpers;
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

        public async Task<List<Report>> GetAllReportsAsync(int pageNumber, int pageSize, int subjectGradeId, string? status = null)
        {
            var query = _dbContext.Reports
                .Where(r => r.Question.Lesson.Chapter.SubjectGradeId == subjectGradeId)
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

        public async Task<int> GetTotalReportsAsync(int subjectGradeId, string? status = null)
        {
            var query = _dbContext.Reports
                .Where(r => r.Question.Lesson.Chapter.SubjectGradeId == subjectGradeId)
                .AsQueryable();

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

        public async Task<Report> GetReportByQuestionAndReason(int questionId, int reason)
        {
            return await _dbContext.Reports.Where(r => r.QuestionId == questionId && r.Reason == (ReportReason)reason).FirstOrDefaultAsync();
        }

        //dashboard-report
        public async Task<int> GetTotalReportCount(int subjectGradeId)
        {
            return await _dbContext.Reports.Where(r => r.Question.Lesson.Chapter.SubjectGradeId == subjectGradeId).SumAsync(r => r.ReportCount);
        }
        //dashboard-report
        public async Task<int> GetTotalReportCountByStatus(string status, int subjectGradeId)
        {
            return await _dbContext.Reports
                .Where(r => r.Status == status && r.Question.Lesson.Chapter.SubjectGradeId == subjectGradeId)
                .SumAsync(r => r.ReportCount);
        }
        //dashboard-report
        public async Task<List<Report>> GetTop5PendingReports(int subjectGradeId)
        {
            return await _dbContext.Reports
                .Where(r => r.Status == "Pending" && r.Question.Lesson.Chapter.SubjectGradeId == subjectGradeId)  
                .OrderByDescending(r => r.ReportCount)  
                .Take(5) 
                .ToListAsync();
        }
        //dashboard-report
        public async Task<(List<string> Labels, List<int> Data)> GetReportStatisticsByReason(int subjectGradeId)
        {
            var result = await _dbContext.Reports
                .Where(r => r.Question.Lesson.Chapter.SubjectGradeId == subjectGradeId)
                .GroupBy(r => r.Reason) 
                .Select(group => new
                {
                    Reason = EnumHelper.GetEnumDescription((ReportReason)group.Key), 
                    Count = group.Sum(r => r.ReportCount)
                })
                .ToListAsync();

            List<string> labels = result.Select(x => x.Reason).ToList();
            List<int> data = result.Select(x => x.Count).ToList();

            return (labels, data);
        }
        //dashboard-report
        public async Task<List<Report>> GetReportsValidByDateRange(DateTime fromDate, int subjectGradeId)
        {
            return await _dbContext.Reports
                .Where(r => r.CreatedAt >= fromDate && r.Status == "Resolved" && r.Question.Lesson.Chapter.SubjectGradeId == subjectGradeId)
                .ToListAsync();
        }

        //dashboard-report
        public async Task<int> CountReportsByWeek(int subjectGradeId, int weekOffset)
        {
            var today = DateTime.UtcNow.Date;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek).AddDays(-7 * weekOffset); 
            var endOfWeek = startOfWeek.AddDays(7); 

            var count = await _dbContext.Reports
                .Where(r => r.Question.Lesson.Chapter.SubjectGradeId == subjectGradeId
                            && r.CreatedAt >= startOfWeek
                            && r.CreatedAt < endOfWeek) 
                .CountAsync();

            return count;
        }




    }
}
