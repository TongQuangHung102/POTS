namespace backend.Dtos
{
    public class ChapterDto
    {
        public int ChapterId { get; set; }
        public string ChapterName { get; set; }
        public int Semester { get; set; }
        public int Order { get; set; }
        public bool IsVisible { get; set; }
    }
}
