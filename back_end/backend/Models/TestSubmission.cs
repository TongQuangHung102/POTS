using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class TestSubmission
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SubmissionId { get; set; }
        public int? StudentAnswer { get; set; }

        [ForeignKey("StudentTest")]
        public int StudentTestId { get; set; }
        public virtual StudentTest StudentTest { get; set; } 
        [ForeignKey("Question")]
        public int QuestionId { get; set; }
        public virtual Question Question { get; set; }
    }
}
