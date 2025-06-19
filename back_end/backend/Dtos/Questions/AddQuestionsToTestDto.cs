namespace backend.Dtos.Questions
{
    public class AddQuestionsToTestDto
    {
        public int TestId { get; set; }
        public List<int> QuestionIds { get; set; }
    }
}
