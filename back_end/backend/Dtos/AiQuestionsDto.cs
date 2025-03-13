namespace backend.Dtos
{
    public class AiQuestionsDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public int LevelId { get; set; }
        public int CorrectAnswer { get; set; }
        public string Status { get; set; }
        public List<AIAnswer> AnswerQuestions { get; set; }
    }

    public class AIQuestionResponse
    {
        public List<AIQuestion> Questions { get; set; }
    }

    public class AIQuestion
    {
        public string QuestionText { get; set; }
        public int LevelId { get; set; }
        public int CorrectAnswer { get; set; }
        public List<AIAnswer> AnswerQuestions { get; set; }
    }

    public class AIAnswer
    {
        public string AnswerText { get; set; }
        public int Number { get; set; }
    }

}
