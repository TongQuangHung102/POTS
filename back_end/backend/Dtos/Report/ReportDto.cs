using backend.Dtos.Questions;
using backend.Models;

namespace backend.Dtos.Report
{
    public class ReportDto
    {
        public int QuestionId { get; set; }
        public int UserId { get; set; }
        public ReportReason Reason { get; set; }
    }
    public class ReportEditDto
    {
        public int ReportId { get; set; }
        public int QuestionId { get; set; }
        public string Status { get; set; }
        public QuestionReportDto Question { get; set; }
    }

    public class QuestionReportDto
    {
        public int CorrectAnswer { get; set; }
        public string QuestionText { get; set; }
        public List<AnswerQuestionDto> AnswerQuestions { get; set; }
    }

}
