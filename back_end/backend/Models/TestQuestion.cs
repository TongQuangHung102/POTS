using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class TestQuestion
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int TestQuestionId { get; set; }
        [ForeignKey("Question")]
        public int QuestionId { get; set; }
        public virtual Question Question { get; set; } 
        [ForeignKey("Test")]
        public int TestId { get; set; }
        public virtual Test Test { get; set; } 
    }
}
