namespace backend.Dtos.PracticeAndTest
{
    public class PracticeAttemptDto
    {
        public int CorrectAnswers { get; set; }
        public int Level { get; set; }
        public double TimePractice { get; set; }
        public int UserId { get; set; }
        public int LessonId { get; set; }
        public string? SampleQuestion { get; set; }
        public List<StudentAnswerDto> Answers { get; set; }
    }

    public class StudentAnswerDto
    {
        public int QuestionId { get; set; }
        public int SelectedAnswer { get; set; }
    }
}
