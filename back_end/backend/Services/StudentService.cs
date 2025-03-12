using backend.Dtos;
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

        public async Task<StudentDashboardDto> GetDashboardDataAsync(int userId)
        {
            var today = DateTime.Today;
            var rate = "";

            var (rank, percentiles) = await _studentPerformanceService.GetStudentRankAndPercentileAsync(userId);
            var student = await _userRepository.GetAllInfomationUser(userId);


            var totalPractice = await _practiceRepository.GetTotalNumberPracticeAsync(userId);
            var avg_score = await _practiceRepository.GetAveragePracticeScoreAsync(userId);
            var avg_time = await _practiceRepository.GetAveragePracticeTimeAsync (userId);

            if (avg_score < 5) rate = "Yếu";
            if (avg_score >= 5 && avg_score < 6.5) rate = "Trung bình";
            if (avg_score >= 6.5 && avg_score < 8.5 ) rate = "Khá";
            if (avg_score >= 8.5) rate = "Giỏi";

            var activityLabels = new List<string>();
            var activityValues = new List<double>();

            for (int i = 6; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                activityLabels.Add(date.ToString("dd/MM"));
                var practiceTime = await _practiceRepository.GetTotalPracticeTimeByDateAsync(userId, date);
                activityValues.Add(practiceTime);
            }


            return new StudentDashboardDto
            {
                PracticeNumber = totalPractice,
                Avg_score = Math.Round(avg_score, 2),
                Avg_time = avg_time / 10, 
                Rate = rate,
                Rank = rank,
                Percentiles = percentiles,
                Activity = new ActivityDto
                {
                    Labels = activityLabels,
                    Data = activityValues
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
