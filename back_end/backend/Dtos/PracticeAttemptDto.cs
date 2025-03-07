namespace backend.Dtos
{
    public class PracticeAttemptDto
    {
        public int CorrectAnswers { get; set; }
        public int Level { get; set; }
        public TimeSpan Time { get; set; }
        public int UserId { get; set; }
        public int LessonId { get; set; }
    }
}
