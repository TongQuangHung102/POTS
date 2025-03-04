namespace backend.Dtos
{
    public class TestQuestionDto
    {
        public int TestQuestionId { get; set; }
        public int TestId { get; set; }
        public string TestName { get; set; }
        public string TestDescription { get; set; }
        public int DurationInMinutes { get; set; }  
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int CorrectAnswer { get; set; }
        public bool IsVisible { get; set; }
        public List<AnswerQuestionDto> Answers { get; set; }
    }
}
