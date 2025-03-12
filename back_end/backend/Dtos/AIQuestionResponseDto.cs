using System.Text.Json.Serialization;

namespace backend.Dtos
{
    public class AIQuestionResponseDto
    {
        [JsonPropertyName("questions")]
        public List<AIQuestionDetailDto> Questions { get; set; }
    }

    public class AIQuestionDetailDto
    {
        [JsonPropertyName("questionText")]
        public string QuestionText { get; set; }

        [JsonPropertyName("levelId")]
        public int LevelId { get; set; }

        [JsonPropertyName("correctAnswer")]
        public int CorrectAnswer { get; set; }

        [JsonPropertyName("answerQuestions")]
        public List<AnswerDto> AnswerQuestions { get; set; }
    }

    public class AnswerDto
    {
        [JsonPropertyName("answerText")]
        public string AnswerText { get; set; }

        [JsonPropertyName("number")]
        public int Number { get; set; }
    }
}
