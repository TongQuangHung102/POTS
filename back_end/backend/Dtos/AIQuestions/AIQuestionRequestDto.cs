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
    }
}
