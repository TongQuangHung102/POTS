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
        public async Task<(int Rank, List<double> Percentiles)> GetStudentRankAndPercentileAsync(int userId)
        {
            var performances = await _studentPerformance.GetAverageStudentPerformanceByLevel();

            if (performances.Count == 0)
                return (-1, new List<double> { 0, 0 });

            var rankings = performances
                .Select(sp => new
                {
                    sp.UserId,
                    Score = (0.7 * sp.avg_Accuracy) - (0.3 * sp.avg_Time)
                })
                .OrderByDescending(sp => sp.Score) 
                .ToList();

            int rank = rankings.FindIndex(sp => sp.UserId == userId) + 1;
            if (rank == 0)
                return (-1, new List<double> { 0, 0 });

            int totalStudents = rankings.Count;
            double lowerPercent = ((totalStudents - rank) / (double)totalStudents) * 100;
            double higherPercent = (rank / (double)totalStudents) * 100;

            return (rank, new List<double> { lowerPercent, higherPercent });
        }

    }
}
