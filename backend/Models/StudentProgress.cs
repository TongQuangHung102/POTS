using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class StudentProgress
    {
        public int UserId { get; set; }
        public int LessonId { get; set; }
        public string ProgressStatus { get; set; }
        public DateTime LastUpdate { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("LessonId")]
        public virtual Lesson Lesson { get; set; }
    }
}
