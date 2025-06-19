using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class StudentProgress
    {
        public int UserId { get; set; }
        public int LessonId { get; set; }
        public double ProgressPercent { get; set; }
        public DateTime LastUpdate { get; set; }
        public bool IsCompleted { get; set; }
        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("LessonId")]
        public virtual Lesson Lesson { get; set; }
    }
}
