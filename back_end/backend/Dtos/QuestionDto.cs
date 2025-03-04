namespace backend.Dtos
{
    public class QuestionDto
    {

        public string QuestionText { get; set; }
        public DateTime CreateAt { get; set; }
        public int LevelId { get; set; }
        public int CorrectAnswer { get; set; }
        public bool IsVisible { get; set; }
        public bool CreateByAI { get; set; }
        public int LessonId { get; set; }
        public List<AnswerQuestionDto> AnswerQuestions { get; set; }
    }
    public class AnswerQuestionDto
    {
        public int AnswerQuestionId { get; set; }
        public string AnswerText { get; set; }
        public int Number { get; set; } 
    }
    public class CreateQuestionDto
    {
        public string QuestionText { get; set; }
        public DateTime CreateAt { get; set; }
        public int LevelId { get; set; }
        public int CorrectAnswer { get; set; }
        public bool IsVisible { get; set; }
        public bool CreateByAI { get; set; }
        public int LessonId { get; set; }
        public List<CreateAnswerDto> AnswerQuestions { get; set; }
    }
    public class CreateAnswerDto
    {
        public string AnswerText { get; set; }
        public int Number { get; set; }
    }

    public class QuestionManageDto
    {

    }
}
