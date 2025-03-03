using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Grades
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int GradeId { get; set; }
        public string GradeName { get; set; }
        public string Description { get; set; }
        public bool IsVisible { get; set; }
        public int? UserId { get; set; }
        [ForeignKey("UserId")]
        public virtual User? User { get; set; }
        public virtual ICollection<Chapter> Chapters { get; set; }
        public virtual ICollection<Test> Tests { get; set; }
        public virtual ICollection<User> Users { get; set; }
    }
}
