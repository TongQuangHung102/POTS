using backend.Dtos;
using backend.Dtos.Dashboard;
using backend.Repositories;

namespace backend.Services
{
    public class ContentManageService
    {
        private readonly IUserRepository _userRepository;
        private readonly ICurriculumRepository _curriculumRepository;
        private readonly IQuestionRepository _questionRepository;
        private readonly IPracticeRepository _practiceRepository;
        private readonly IAIQuestionRepository _aiQuestionRepository;
        private readonly ITestRepository _testRepository;
        private readonly ISubjectGradeRepository _subjectGradeRepository;

        public ContentManageService(IUserRepository userRepository, ICurriculumRepository curriculumRepository, IQuestionRepository questionRepository, IPracticeRepository practiceRepository, IAIQuestionRepository aiQuestionRepository, ITestRepository testRepository, ISubjectGradeRepository subjectGradeRepository)
        {
            _userRepository = userRepository;
            _curriculumRepository = curriculumRepository;
            _questionRepository = questionRepository;
            _practiceRepository = practiceRepository;
            _aiQuestionRepository = aiQuestionRepository;
            _testRepository = testRepository;
            _subjectGradeRepository = subjectGradeRepository;
        }

        public async Task<ContentManageDto> GetContentManageDashboardData(int gradeId, int subjectId)
        {
            try
            {
                var subjectGrade = await _subjectGradeRepository.GetByGradeAndSubjectAsync(gradeId, subjectId);
                var subjectTest = await _subjectGradeRepository.GetTestByGradeAndSubjectAsync(gradeId, subjectId);
                var today = DateTime.Today;

                var totalStudent = await _userRepository.GetTotalUsersAsync(1, null, gradeId);
                var newStudent = await _userRepository.GetTotalNewStudent(3, gradeId);
                var totalQuestion = await _questionRepository.CountQuestionInGrade(gradeId);
                var totalAIQuestion = await _aiQuestionRepository.CountQuestionAIInGrade(gradeId);

                var totalStudentLabels = new List<string>();
                var totalStudentValues = new List<int>();

                var totalTimeLabels = new List<string>();
                var totalTimeValues = new List<double>();

                for (int i = 7; i >= 0; i--)
                {
                    var date = today.AddDays(-i);
                    totalTimeLabels.Add(date.ToString("dd/MM"));
                    var totalTimeChart = await _practiceRepository.GetTotalPracticeTimeAllStudentByDateAsync(date, gradeId, subjectId);
                    totalTimeValues.Add(totalTimeChart);
                }

                for (int i = 7; i >= 0; i--)
                {
                    var date = today.AddDays(-i);
                    totalStudentLabels.Add(date.ToString("dd/MM"));
                    var totalStudentChart = await _userRepository.GetTotalStudentByDate(date, gradeId);
                    totalStudentValues.Add(totalStudentChart);
                }

                var listChapter = subjectGrade?.Chapters?.Take(3).Select(c => new ChapterDashboard
                {
                    Id = c.ChapterId,
                    Name = c.ChapterName,
                    Order = c.Order
                }) ?? new List<ChapterDashboard>();

                var listTest = subjectTest?.Tests?.Take(3).Select(t => new TestDashboard
                {
                    Id = t.TestId,
                    TestName = t.TestName
                }) ?? new List<TestDashboard>();

                return new ContentManageDto
                {
                    TotalStudent = totalStudent,
                    NewStudent = newStudent,
                    TotalQuestion = totalQuestion,
                    TotalQuestionAi = totalAIQuestion,
                    Chapters = listChapter.ToList(),
                    TestDashboards = listTest.ToList(),
                    TotalStudentDto = new TotalStudentDto
                    {
                        Labels = totalStudentLabels,
                        Data = totalStudentValues
                    },
                    ActivityDto = new ActivityDto
                    {
                        Labels = totalTimeLabels,
                        Data = totalTimeValues
                    }
                };
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy dữ liệu Dashboard!"); 
            }
        }

    }
}
