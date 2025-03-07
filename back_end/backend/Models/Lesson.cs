using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Lesson
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LessonId { get; set; }
        public string LessonName { get; set; }
        public int Order { get; set; }
        public bool IsVisible { get; set; }
        public int ChapterId { get; set; }

        [ForeignKey("ChapterId")]
        public virtual Chapter Chapter { get; set; }
        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<AIQuestion> AIQuestions { get; set; }
        public virtual ICollection<PracticeAttempt> PracticeAttempts { get; set; }
        public virtual ICollection<StudentPerformance> StudentPerformances { get; set; }
        public virtual ICollection<StudentProgress> StudentProgresses { get; set; }
    }
}
