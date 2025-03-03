namespace backend.Dtos
{
    public class TestDto
    {
        public int TestId { get; set; }
        public string TestName { get; set; }
        public string Description { get; set; }
        public int DurationInMinutes { get; set; }
        public int MaxScore { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsVisible { get; set; }
        public int Order { get; set; }
        public int GradeId { get; set; }
    }
}
