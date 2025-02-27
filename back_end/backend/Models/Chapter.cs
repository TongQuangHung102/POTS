using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Chapter
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ChapterId { get; set; }
        public string ChapterName { get; set; }
        public int Order { get; set; }
        public bool IsVisible { get; set; }
        public int? UserId { get; set; }
        public int GradeId { get; set; }

        public int Semester { get;set; }

        [ForeignKey("UserId")]
        public virtual User? User { get; set; }

        [ForeignKey("GradeId")]
        public virtual Grades Grade { get; set; }
        public virtual ICollection<Lesson> Lessons { get; set; }
        public virtual ICollection<Prerequisite> Prerequisites { get; set; }
    }
}
