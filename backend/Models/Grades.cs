using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Grades
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] 
        public int GradeId { get; set; }
        [Required]
        [MaxLength(255)]
        public string GradeName { get; set; }
        public string Description { get; set; }
        public virtual ICollection<Chapter> Chapters { get; set; }
    }
}
