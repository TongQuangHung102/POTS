using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class StudentTest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int StudentTestId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; } 
        public double? Score { get; set; }
        [ForeignKey("Test")]
        public int TestId { get; set; }
        public virtual Test Test { get; set; }
        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User User { get; set; } 
        public virtual ICollection<TestSubmission> TestSubmissions { get; set; }

    }
}
