namespace backend.Dtos.Dashboard
{
    public class AutoAddQuestionToTestDto
    {
    }

    public class LevelAutoRequest
    {
        public int LevelId { get; set; }
        public int QuestionCount { get; set; }
    }

    public class ChapterQuestionAutoRequest
    {
        public int ChapterId { get; set; }
        public List<LevelAutoRequest> LevelRequests { get; set; }
    }
    public class GenerateTestRequest
    {
        public List<ChapterQuestionAutoRequest> Chapters { get; set; }
    }
}
