using backend.DataAccess.DAO;
using backend.Repositories;

namespace backend.Services
{
    public class StudentPerformanceService
    {
        private readonly IStudentPerformanceRepository _studentPerformance;

        public StudentPerformanceService(IStudentPerformanceRepository studentPerformance)
        {
            _studentPerformance = studentPerformance;
        }
        public async Task<(int Rank, List<double> Percentiles)> GetStudentRankAndPercentileAsync(int userId, int month, int year)
        {
            var performances = await _studentPerformance.GetAverageStudentPerformance(month, year);

            if (performances.Count == 0)
                return (0, new List<double> { 0, 0 });

            var rankings = performances
                .Select(sp => new
                {
                    sp.UserId,
                    Score = (0.5 * sp.avg_Accuracy) - (0.3 * sp.avg_Time) + (0.2 * sp.AttemptCount)
                })
                .OrderByDescending(sp => sp.Score) 
                .ToList();

            int rank = rankings.FindIndex(sp => sp.UserId == userId) + 1;
            if (rank == 0)
                return (0, new List<double> { 0, 0 });

            int totalStudents = rankings.Count;

            if (totalStudents == 1)
                return (1, new List<double> { 100, 0 });

            double lowerPercent = ((totalStudents - rank) * 100.0) / (totalStudents - 1);
            double higherPercent = 100.0 - lowerPercent;

            return (rank, new List<double> { lowerPercent, higherPercent });
        }

    }
}
