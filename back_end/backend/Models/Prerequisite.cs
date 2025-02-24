using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Prerequisite
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Lesson")]
        public int LessonId { get; set; }
        public virtual Lesson Lesson { get; set; }
        [Key, Column(Order = 1)]
        [ForeignKey("Test")]
        public int TestId { get; set; }
        public virtual Test Test { get; set; } 
    }
}
