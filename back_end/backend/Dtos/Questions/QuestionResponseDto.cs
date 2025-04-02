using backend.DataAccess.DAO;
using backend.Dtos.Curriculum;

namespace backend.Dtos.Questions
{
    public class QuestionResponseDto
    {
        public string LessonName { get; set; }
        public int TotalQuestions { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<QuestionDetailDto> Data { get; set; }
    }

    public class QuestionDetailDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public DateTime CreateAt { get; set; }
        public int CorrectAnswer { get; set; }
        public string CorrectAnswerText { get; set; }
        public bool IsVisible { get; set; }
        public bool CreateByAI { get; set; }
        public LevelSimpleDto Level { get; set; }
        public LessonNameDto Lesson { get; set; }
        public List<AnswerQuestionDto> AnswerQuestions { get; set; }
    }

    public class LessonNameDto
    {
        public string LessonName { get; set; }
    }

    public class LevelSimpleDto
    {
        public int LevelId { get; set; }
        public string LevelName { get; set; }
    }
}
