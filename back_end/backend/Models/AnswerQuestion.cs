using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class AnswerQuestion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int AnswerQuestionId { get; set; }
        public string AnswerText { get; set; }
        public int Number { get; set; }
        public int? QuestionId { get; set; }
        public int? QuestionAiId { get; set; }

        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }

        [ForeignKey("QuestionAiId")]
        public virtual AIQuestion AIQuestion { get; set; }
    }
}
