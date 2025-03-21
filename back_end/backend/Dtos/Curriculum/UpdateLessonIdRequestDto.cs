namespace backend.Dtos.Curriculum
{
    public class UpdateLessonIdRequestDto
    {
        public int LessonId { get; set; }
        public List<int> AiQuestionIds { get; set; }
    }
}
