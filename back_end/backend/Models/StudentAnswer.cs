using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class StudentAnswer
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AnswerId { get; set; }
        public int PracticeId { get; set; }
        public int QuestionId { get; set; }
        public int SelectedAnswer { get; set; }

        [ForeignKey("PracticeId")]
        public virtual PracticeAttempt PracticeAttempt { get; set; }

        [ForeignKey("QuestionId")]
        public virtual PracticeQuestion PracticeQuestion { get; set; }
    }
}
