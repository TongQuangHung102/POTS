namespace backend.Dtos.Dashboard
{
    public class ContentManageDto
    {
        public int TotalStudent { get; set; }
        public int NewStudent { get; set; }
        public int TotalQuestion { get; set; }
        public int TotalQuestionAi { get; set; }
        public List<TestDashboard> TestDashboards { get; set; }
        public List<ChapterDashboard> Chapters { get; set; }
        public ActivityDto ActivityDto { get; set; }
        public TotalStudentDto TotalStudentDto { get; set; }

    }

    public class TestDashboard
    {
        public int Id { get; set; }
        public string TestName { get; set; }
    }

    public class ChapterDashboard
    {
        public int Id { get; set; }
        public int Order { get; set; }
        public string Name { get; set; }
    }
}
