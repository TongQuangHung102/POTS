namespace backend.Dtos.Dashboard
{
    public class AdminDashboardDto
    {
        public int TotalStudent { get; set; }
        public int NewStudent { get; set; }
        public int TotalQuestion { get; set; }
        public List<ContentManageAssign> ContentManageAssigns { get; set; }
        public List<SubscriptionPlanDashboardDto> SubscriptionPlanDashboards { get; set; }
        public TotalStudentDto TotalStudentDto { get; set; }
        public ActivityDto ActivityTime { get; set; }
    }

    public class ContentManageAssign
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string GradeAssign { get; set; }
    }

    public class SubscriptionPlanDashboardDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
    }

    public class TotalStudentDto
    {
        public List<string> Labels { get; set; }
        public List<int> Data { get; set; }
    }
}
