using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class PracticeAttempt
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PracticeId { get; set; }
        public int CorrectAnswers { get; set; }
        public int LevelId { get; set; }
        public double TimePractice { get; set; }
        public DateTime CreateAt { get; set; }
        public int UserId { get; set; }
        public int LessonId { get; set; } 
        public string? SampleQuestion { get; set; }

        [ForeignKey("LevelId")]
        public virtual Level Level { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("LessonId")]
        public virtual Lesson Lesson { get; set; }
        public virtual ICollection<StudentAnswer> StudentAnswers { get; set; } 
    }
}
