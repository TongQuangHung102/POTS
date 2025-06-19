using backend.Dtos;
using backend.Dtos.Dashboard;
using backend.Models;
using backend.Repositories;
using System.Text;

namespace backend.Services
{
    public class StudentService
    {
        private readonly IPracticeRepository _practiceRepository;
        private readonly StudentPerformanceService _studentPerformanceService;
        private readonly IUserRepository _userRepository;

        public StudentService(IPracticeRepository practiceRepository, StudentPerformanceService studentPerformanceService, IUserRepository userRepository)
        {
            _practiceRepository = practiceRepository;
            _studentPerformanceService = studentPerformanceService;
            _userRepository = userRepository;
        }

        public async Task<StudentDashboardDto> GetDashboardDataAsync(int userId, int subjectGradeId, int month, int year)
        {
            try
            {
                var today = DateTime.UtcNow.AddHours(7);

                var (rank, percentiles, student, totalPractice, avgScore, avgTime, rate) =
                    await GetStudentSummaryAsync(userId, subjectGradeId, month, year);

                var (startDate, endDate) = GetDateRange(today, month, year);

                var (activityLabelsTime, activityValuesTime, activityLabelsScore, activityValuesScore, activityValuesTime2) =
                    await GetActivityDataAsync(userId, subjectGradeId, startDate, endDate);

                var message = await GenerateMotivationMessageAsync(userId, subjectGradeId, today);

                return new StudentDashboardDto
                {
                    PracticeNumber = totalPractice,
                    Avg_score = Math.Round(avgScore, 2),
                    Avg_time = Math.Round((avgTime / 10), 2),
                    Rate = rate,
                    Rank = rank,
                    Percentiles = percentiles,
                    Message = message,
                    Activity = new ActivityDto
                    {
                        Labels = activityLabelsTime,
                        Data = activityValuesTime
                    },
                    ScoreTime = new ScoreAndTimeDto
                    {
                        Labels = activityLabelsScore,
                        ScoreData = activityValuesScore,
                        TimeData = activityValuesTime2
                    },
                    Student = new StudentDto
                    {
                        Name = student?.UserName ?? "N/A",
                        GradeName = student?.Grade?.GradeName ?? "N/A"
                    }
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Đã xảy ra lỗi khi lấy dữ liệu. Vui lòng thử lại.");
            }
        }

        private async Task<(int rank, List<double> percentiles, User student, int totalPractice, double avgScore, double avgTime, string rate)>
        GetStudentSummaryAsync(int userId, int subjectGradeId, int month, int year)
        {
            var (rank, percentiles) = await _studentPerformanceService.GetStudentRankAndPercentileAsync(userId, month, year);
            var student = await _userRepository.GetAllInfomationUser(userId);

            var totalPractice = await _practiceRepository.GetTotalNumberPracticeAsync(userId, subjectGradeId, month, year);
            var avgScore = await _practiceRepository.GetAveragePracticeScoreAsync(userId, subjectGradeId, month, year);
            var avgTime = await _practiceRepository.GetAveragePracticeTimeAsync(userId, subjectGradeId, month, year);

            string rate = avgScore switch
            {
                < 5 => "Yếu",
                >= 5 and < 7 => "Trung bình",
                >= 7 and < 9 => "Khá",
                _ => "Giỏi"
            };

            return (rank, percentiles, student, totalPractice, avgScore, avgTime, rate);
        }

        private async Task<(List<string> activityLabelsTime, List<double> activityValuesTime,
                        List<string> activityLabelsScore, List<double> activityValuesScore, List<double> activityValuesTime2)>
        GetActivityDataAsync(int userId, int subjectGradeId, DateTime startDate, DateTime endDate)
        {
            var activityLabelsTime = new List<string>();
            var activityValuesTime = new List<double>();
            var activityLabelsScore = new List<string>();
            var activityValuesScore = new List<double>();
            var activityValuesTime2 = new List<double>();

            for (var date = startDate; date <= endDate; date = date.AddDays(1))
            {
                var dateString = date.ToString("dd/MM");

                activityLabelsTime.Add(dateString);
                activityValuesTime.Add(await _practiceRepository.GetTotalPracticeTimeByDateAsync(subjectGradeId, userId, date));

                activityLabelsScore.Add(dateString);
                activityValuesScore.Add(await _practiceRepository.GetAverageScoreByDateAsync(subjectGradeId, userId, date));
                activityValuesTime2.Add(await _practiceRepository.GetAverageTimeByDateAsync(subjectGradeId, userId, date));
            }

            return (activityLabelsTime, activityValuesTime, activityLabelsScore, activityValuesScore, activityValuesTime2);
        }

        private (DateTime startDate, DateTime endDate) GetDateRange(DateTime today, int month, int year)
        {
            DateTime startDate, endDate;

            if (month == today.Month && year == today.Year)
            {
                startDate = today.Day < 7 ? today.AddDays(-6) : new DateTime(year, month, 1);
                endDate = today;
            }
            else
            {
                startDate = new DateTime(year, month, 1);
                endDate = new DateTime(year, month, DateTime.DaysInMonth(year, month));
            }

            return (startDate, endDate);
        }

        private async Task<string> GenerateMotivationMessageAsync(int userId, int subjectGradeId, DateTime today)
        {
            var yesterday = today.AddDays(-1);
            var twoDaysAgo = today.AddDays(-2);

            // Lấy điểm trung bình
            var avgScoreYesterday = await _practiceRepository.GetAverageScoreByDateAsync(subjectGradeId, userId, yesterday);
            var avgScoreTwoDaysAgo = await _practiceRepository.GetAverageScoreByDateAsync(subjectGradeId, userId, twoDaysAgo);


            var motivationMessage = "";

            if (avgScoreYesterday > avgScoreTwoDaysAgo)
            {
                motivationMessage = "🎉 Hôm qua bạn làm bài rất tốt, hãy giữ vững phong độ nhé!";
            }
            else if (avgScoreYesterday < avgScoreTwoDaysAgo)
            {
                motivationMessage = "📉 Hôm qua bạn làm chưa được tốt lắm, hãy cố gắng hơn nhé!";
            }
            else if (avgScoreYesterday == 0)
            {
                motivationMessage = "📢 Hôm qua bạn chưa luyện tập, hãy chăm chỉ hơn nhé!";
            }
            else
            {
                motivationMessage = "⚖️ Hãy cố gắng hơn hôm qua nhé!";
            }


            return motivationMessage;
        }


    }
}
