namespace backend.Dtos.Curriculum
{
    public class LessonDto
    {
        public int LessonId { get; set; }
        public string LessonName { get; set; }
        public int Order { get; set; }
        public bool IsVisible { get; set; }
    }
}
