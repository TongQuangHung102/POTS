using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Level
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int LevelId { get; set; }
        public string LevelName { get; set; }
        public string LevelDescription { get; set; }
        public int LevelNumber { get; set; }

        public virtual ICollection<Question> Questions { get; set; }
        public virtual ICollection<AIQuestion> AIQuestions { get; set; }
        public virtual ICollection<StudentPerformance> StudentPerformance { get; set; }
        public virtual ICollection<PracticeAttempt> PracticeAttempta { get; set; }
    }
}
