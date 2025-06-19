namespace backend.Dtos.Subject
{
    public class SubjectDto
    {
        public string SubjectName { get; set; }
        public bool IsVisible { get; set; }
    }

    public class SubjectEditDto
    {
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public bool IsVisible { get; set; }
    }
}
