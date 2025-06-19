using backend.Dtos.AIQuestions;

namespace backend.Dtos.PracticeQuestion
{
    public class PracticeQuestionDto
    {
        public string questionText { get; set; }
        public int correctAnswer { get; set; }
        public List<AnswerDTO> answerQuestions { get; set; }
    }

    public class AnswerDTO
    {
        public string answerText { get; set; }
        public int number { get; set; }
        public int questionId { get; set; }
    }


}
