namespace backend.Dtos
{
    public class AssignContentManagerRequest
    {

        public List<ChapterAssignment> Assignments { get; set; }
    }
    public class ChapterAssignment
    {
        public int ChapterId { get; set; }
        public int UserId { get; set; }
    }
}
