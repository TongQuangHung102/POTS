namespace backend.Dtos.Curriculum
{
    public class ChapterWithQuestionLevelsDto
    {
        public int ChapterId { get; set; }
        public string ChapterName { get; set; }
        public int Order { get; set; }
        public int Semester { get; set; }
        public bool IsVisible { get; set; }
        public Dictionary<int, int> QuestionLevelCounts { get; set; } = new();
    }
}
