using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Test
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int TestId { get; set; }
        public string TestName { get; set; }
        public string Description { get; set; }
        public TimeSpan Duration { get; set; }
        public int MaxScore { get; set; }
        public DateTime CreatedAt { get; set; }
        public bool IsVisible { get; set; }
        public virtual ICollection<TestQuestion> TestQuestions { get; set; }
        public virtual ICollection<StudentTest> StudentTests { get; set; }
        public virtual ICollection<Prerequisite> Prerequisites { get; set; }
    }
}
