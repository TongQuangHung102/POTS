using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class StudentAnswer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AnswerId { get; set; }
        public int AttemptId { get; set; }
        public int QuestionId { get; set; }
        public int SelectedAnswer { get; set; }

        [ForeignKey("AttemptId")]
        public virtual PracticeAttempt PracticeAttempts { get; set; }

        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }
    }
}
