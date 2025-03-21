using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Question
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int QuestionId { get; set; }
        public string QuestionText { get; set; }
        public DateTime CreateAt { get; set; }
        [ForeignKey("LevelId")]
        public int LevelId { get; set; }
        public int CorrectAnswer { get; set; }
        public bool IsVisible { get; set; }
        public bool CreateByAI { get; set; }
        public int LessonId { get; set; }

        [ForeignKey("LessonId")]
        public virtual Lesson Lesson { get; set; }
        public virtual Level Level { get; set; }
        public virtual ICollection<AnswerQuestion> AnswerQuestions { get; set; }
        public virtual ICollection<ContestQuestion> ContestQuestions { get; set; }
        public virtual ICollection<TestQuestion> TestQuestions { get; set; }
        public virtual ICollection<TestSubmission> TestSubmissions { get; set; }
        public virtual ICollection<StudentAnswer> StudentAnswers { get; set; }
        public virtual ICollection<Report> Reports { get; set; }

    }
}
