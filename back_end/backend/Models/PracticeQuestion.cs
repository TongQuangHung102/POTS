using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class PracticeQuestion
    {

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public DateTime CreateAt { get; set; }
        public int CorrectAnswer { get; set; }
        public int LessonId { get; set; }

        [ForeignKey("LessonId")]
        public virtual Lesson Lesson { get; set; }
        public virtual ICollection<AnswerPracticeQuestion> AnswerPracticeQuestions { get; set; }
        public virtual ICollection<StudentAnswer> StudentAnswers { get; set; }

    }
}
