using backend.Dtos;
using backend.Dtos.Dashboard;
using backend.Models;
using backend.Repositories;

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

        public async Task<StudentDashboardDto> GetDashboardDataAsync(int userId, int subjectGradeId)
        {
            var today = DateTime.Today;
            var rate = "";

            var (rank, percentiles) = await _studentPerformanceService.GetStudentRankAndPercentileAsync(userId);
            var student = await _userRepository.GetAllInfomationUser(userId);


            var totalPractice = await _practiceRepository.GetTotalNumberPracticeAsync(subjectGradeId, userId);
            var avg_score = await _practiceRepository.GetAveragePracticeScoreAsync(subjectGradeId, userId);
            var avg_time = await _practiceRepository.GetAveragePracticeTimeAsync(subjectGradeId, userId);

            if (avg_score < 5) rate = "Yếu";
            if (avg_score >= 5 && avg_score < 6.5) rate = "Trung bình";
            if (avg_score >= 6.5 && avg_score < 8.5 ) rate = "Khá";
            if (avg_score >= 8.5) rate = "Giỏi";

            var activityLabelsTime = new List<string>();
            var activityValuesTime = new List<double>();
            var activityLabelsScore = new List<string>();
            var activityValuesScore = new List<double>();
            var activityValuesTime2 = new List<double>();

            for (int i = 6; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                activityLabelsTime.Add(date.ToString("dd/MM"));
                var practiceTime = await _practiceRepository.GetTotalPracticeTimeByDateAsync(subjectGradeId, userId,  date);
                activityValuesTime.Add(practiceTime);
            }

            for (int i = 6; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                activityLabelsScore.Add(date.ToString("dd/MM"));
                var practiceScore = await _practiceRepository.GetAverageScoreByDateAsync(subjectGradeId,userId, date);
                var practiceTime = await _practiceRepository.GetAverageTimeByDateAsync(subjectGradeId, userId, date);
                activityValuesScore.Add(practiceScore);
                activityValuesTime2.Add(practiceTime);

            }


            return new StudentDashboardDto
            {
                PracticeNumber = totalPractice,
                Avg_score = Math.Round(avg_score, 2),
                Avg_time = Math.Round((avg_time / 10), 2) , 
                Rate = rate,
                Rank = rank,
                Percentiles = percentiles,
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
                    Name = student.UserName,
                    GradeName = student.Grade.GradeName
                }
            };

        }
    }
}
