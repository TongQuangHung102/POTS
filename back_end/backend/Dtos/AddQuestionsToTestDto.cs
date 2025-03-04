namespace backend.Dtos
{
    public class AddQuestionsToTestDto
    {
        public int TestId { get; set; }
        public List<int> QuestionIds { get; set; }
    }
}
