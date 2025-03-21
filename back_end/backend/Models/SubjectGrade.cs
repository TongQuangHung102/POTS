using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics;

namespace backend.Models
{
    public class SubjectGrade
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        [ForeignKey("Subject")]
        public int SubjectId { get; set; }
        public Subject Subject { get; set; }

        [ForeignKey("Grade")]
        public int GradeId { get; set; }
        public Grades Grade { get; set; }
        public virtual ICollection<Chapter> Chapters { get; set; }
        public virtual ICollection<Test> Tests { get; set; }
    }
}
