namespace backend.Dtos.Questions
{
    public class QuestionRequest
    {
        public int userId { get; set; }
        public int lessonId { get; set; }

    }

    public class PracticeResults
    {
        public int num_correct { get; set; }
        public int total_questions { get; set; }
        public int time_taken { get; set; }
    }

    public class PracticeSession
    {
        public string question { get; set; }
        public int num_questions { get; set; }
        public PracticeResults results { get; set; }
    }
}
