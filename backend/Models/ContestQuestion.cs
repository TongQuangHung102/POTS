using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class ContestQuestion
    {
        [Key]
        public int ContestQuestionId { get; set; }
        public int ContestId { get; set; }
        public int QuestionId { get; set; }

        [ForeignKey("ContestId")]
        public virtual Contest Contest { get; set; }

        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }
    }
}
