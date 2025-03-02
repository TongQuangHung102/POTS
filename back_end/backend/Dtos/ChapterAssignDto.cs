namespace backend.Dtos
{
    public class ChapterAssignDto
    {
        public int ChapterId { get; set; }
        public string ChapterName { get; set; }
        public bool IsVisible { get; set; }
        public int Order { get; set; }
        public int Semester { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string? GradeName { get; set; }
    }
}
