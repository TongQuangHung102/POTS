using backend.Dtos;
using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class AdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;
        private readonly IPracticeRepository _practiceRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IGradeRepository _gradeRepository;

        public AdminService(IUserRepository userRepository, ISubscriptionPlanRepository subscriptionPlanRepository, IPracticeRepository practiceRepository, IQuestionRepository questionRepository, IGradeRepository gradeRepository)
        {
            _userRepository = userRepository;
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _practiceRepository = practiceRepository;
            _questionRepository = questionRepository;
            _gradeRepository = gradeRepository;
        }

        public async Task<AdminDashboardDto> GetAdminDashboardData()
        {
            var today = DateTime.Today;

            var totalStudent = await _userRepository.GetTotalUsersAsync(1, null);
            var newStudent = await _userRepository.GetTotalNewStudent(3);
            var totalQuestion = await _questionRepository.GetTotalQuestionsAsync(null, null, null, null, null);
            var subscriptionPlans = await _subscriptionPlanRepository.GetAllAsync();
            var grades = await _gradeRepository.GetAllGradesAsync();

            var totalStudentLabels = new List<string>();
            var totalStudentValues = new List<int>();

            var totalTimeLabels = new List<string>();
            var totalTimeValues = new List<double>();


            for (int i = 7; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                totalTimeLabels.Add(date.ToString("dd/MM"));
                var totalTimeChart = await _practiceRepository.GetTotalPracticeTimeAllStudentByDateAsync(date);
                totalTimeValues.Add(totalTimeChart);
            }

            for (int i = 7; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                totalStudentLabels.Add(date.ToString("dd/MM"));
                var totalStudentChart = await _userRepository.GetTotalStudentByDate(date);
                totalStudentValues.Add(totalStudentChart);
            }

            var listSubscription = subscriptionPlans.Select(sp => new SubscriptionPlanDashboardDto
            {
                Name = sp.PlanName,
                Price = sp.Price
            });
            var listGrade = grades.Select(g => new ContentManageAssign
            {
                Name = g.User.UserName,
                Email = g.User.Email,
                GradeAssign = g.GradeName
            });

            return new AdminDashboardDto
            {
                TotalStudent = totalStudent,
                TotalQuestion = totalQuestion,
                NewStudent = newStudent,
                ContentManageAssigns = listGrade.ToList(),
                SubscriptionPlanDashboards = listSubscription.ToList(),
                TotalStudentDto = new TotalStudentDto
                {
                    Labels = totalStudentLabels,
                    Data = totalStudentValues
                },
                ActivityTime = new ActivityDto
                {
                    Labels = totalTimeLabels,
                    Data = totalTimeValues
                }
            };
        }
    }
}
