using Newtonsoft.Json;

namespace backend.Dtos.SubjectGrade
{
    public class AIFileProcessingResponseDto
    {
        [JsonProperty("collection_name")]
        public string CollectionName { get; set; }

        [JsonProperty("documents_processed")]
        public int DocumentsProcessed { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }
    }
}
