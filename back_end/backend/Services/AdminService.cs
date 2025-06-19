using backend.Dtos;
using backend.Dtos.Dashboard;
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
        private readonly IUserParentStudentRepository _userParentStudentRepository;
        private readonly IPaymentRepository _paymentRepository;

        public AdminService(IUserRepository userRepository, ISubscriptionPlanRepository subscriptionPlanRepository, IPracticeRepository practiceRepository, IQuestionRepository questionRepository, IGradeRepository gradeRepository, IUserParentStudentRepository userParentStudentRepository, IPaymentRepository paymentRepository)
        {
            _userRepository = userRepository;
            _subscriptionPlanRepository = subscriptionPlanRepository;
            _practiceRepository = practiceRepository;
            _questionRepository = questionRepository;
            _gradeRepository = gradeRepository;
            _userParentStudentRepository = userParentStudentRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<AdminDashboardDto> GetAdminDashboardData(int? gradeId)
        {
            var today = DateTime.UtcNow.AddHours(7).Date;

            var totalStudent = await _userRepository.GetTotalUsersAsync(1, null, gradeId);
            var totalParent = await _userParentStudentRepository.GetParentCountAsync(gradeId);
            var newStudent = await _userRepository.GetTotalNewStudent(3, gradeId);
            var totalAmount = await _paymentRepository.GetTotalRevenueAsync(gradeId);
            var subscriptionPlans = await _subscriptionPlanRepository.GetAllAsync();
            var grades = await _gradeRepository.GetAllGradesAsync();

            var totalStudentLabels = new List<string>();
            var totalStudentValues = new List<int>();

            var totalAmountLabels = new List<string>();
            var totalAmounteValues = new List<decimal>();


            for (int i = 7; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                totalAmountLabels.Add(date.ToString("dd/MM"));
                var totalAmountChart = await _paymentRepository.GetDailyRevenueAsync(date, gradeId);
                totalAmounteValues.Add(totalAmountChart);
            }

            for (int i = 7; i >= 0; i--)
            {
                var date = today.AddDays(-i);
                totalStudentLabels.Add(date.ToString("dd/MM"));
                var totalStudentChart = await _userRepository.GetTotalStudentByDate(date, gradeId);
                totalStudentValues.Add(totalStudentChart);
            }

            var listSubscription = subscriptionPlans.Select(sp => new SubscriptionPlanDashboardDto
            {
                Name = sp.PlanName,
                Price = sp.Price
            });
            var listGrade = grades.Where(g => g.IsVisible == true).Select(g => new ContentManageAssign
            {
                Name = g.User?.UserName ?? "N/A", 
                Email = g.User?.Email ?? "N/A",
                GradeAssign = g.GradeName
            });

            return new AdminDashboardDto
            {
                TotalStudent = totalStudent,
                TotalAmount = totalAmount,
                TotalParent = totalParent,
                NewStudent = newStudent,
                ContentManageAssigns = listGrade.ToList(),
                SubscriptionPlanDashboards = listSubscription.ToList(),
                TotalStudentDto = new TotalStudentDto
                {
                    Labels = totalStudentLabels,
                    Data = totalStudentValues
                },
                TotalAmountDto = new TotalAmountDto
                {
                    Labels = totalAmountLabels,
                    Data = totalAmounteValues
                }
            };
        }
    }
}
