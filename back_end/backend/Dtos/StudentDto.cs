namespace backend.Dtos
{
    public class StudentDto
    {
        public string Name { get; set; }
        public string GradeName { get; set; }
        public string SubscriptionName { get; set; }
        public string Email { get; set; }
    }
    public class StudentChapterDto
    {
        public int ChapterId { get; set; }
        public string Name { get; set; }
        public int Order { get; set; }
        public List<StudentLessonDto> Lessons { get; set; }
    }

    public class StudentLessonDto
    {
        public int LessonId { get; set; }
        public string LessonName { get; set; }
        public int Order { get; set; }
        public double? AverageScore { get; set; }
        public double? AverageTime { get; set; }
    }
}
