namespace backend.Dtos.Report
{
    public class ReportDashboard
    {
        public int TotalReport { get; set; }
        public int ValidReport { get; set; }
        public int InValidReport { get; set; }
        public int PendingReport { get; set; }
        public TotalReportByReasonDto TotalReportByReason { get; set; }
        public List<ReportInDashboard> ReportInDashboards { get; set; }
    }

    public class TotalReportByReasonDto
    {
        public List<string> Labels { get; set; }
        public List<int> Data { get; set; }
    }

    public class ReportInDashboard
    {
        public int Id { get; set; }
        public string Reason { get; set; }
        public int ReportCount { get; set; }
        public int QuestionId { get; set; }
        public string Status { get; set; }

    }
}
