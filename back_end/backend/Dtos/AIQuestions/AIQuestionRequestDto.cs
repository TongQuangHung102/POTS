using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace backend.Dtos.AIQuestions
{
    public class AIQuestionRequestDto
    {

        [JsonProperty("question")]
        [Required]
        public string Question { get; set; }
        [JsonProperty("num_questions")]
        [Required]
        public int NumQuestions { get; set; }
        [JsonProperty("collection_name")]
        [Required]
        public string CollectionName { get; set; }
    }

    public class AIQuestionRequestCreate
    {
        public int LessonId { get; set; }
        public int Num { get; set; }
    }
}
