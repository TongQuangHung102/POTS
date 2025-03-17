namespace backend.Dtos
{
    public class StudentDashboardDto
    {
        public int PracticeNumber { get; set; }
        public double Avg_score { get; set; }
        public double Avg_time { get; set; }
        public string Rate { get; set; }
        public int Rank { get; set; }
        public List<double> Percentiles { get; set; }
        public ActivityDto Activity { get; set; }
        public ScoreAndTimeDto ScoreTime { get; set; }
        public StudentDto Student { get; set; }
    }


    public class ActivityDto
    {
        public List<string> Labels { get; set; } 
        public List<double> Data { get; set; } 
    }

    public class ScoreAndTimeDto
    {
        public List<string> Labels { get; set; }
        public List<double> ScoreData { get; set; }
        public List<double> TimeData { get; set; }
    }
}
