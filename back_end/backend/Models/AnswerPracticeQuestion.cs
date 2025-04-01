using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class AnswerPracticeQuestion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AnswerQuestionId { get; set; }
        public string AnswerText { get; set; }
        public int Number { get; set; }
        public int QuestionId { get; set; }

        [ForeignKey("QuestionId")]
        public virtual PracticeQuestion PracticeQuestion { get; set; }
    }
}
