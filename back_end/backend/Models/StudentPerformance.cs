using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class StudentPerformance
    {
        public int UserId { get; set; }
        public int LessonId { get; set; }
        public double? avg_Accuracy { get; set; }
        public double? avg_Time { get; set; }
        public DateTime LastAttempt { get; set; }
        [ForeignKey("LevelId")]
        public int LevelId { get; set; }
        public virtual Level Level { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("LessonId")]
        public virtual Lesson Lesson { get; set; }
    }
}
