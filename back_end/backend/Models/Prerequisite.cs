using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Prerequisite
    {
        [Key, Column(Order = 0)]
        [ForeignKey("Chapter")]
        public int ChapterId { get; set; }
        public virtual Chapter Chapter { get; set; }
        [Key, Column(Order = 1)]
        [ForeignKey("Test")]
        public int TestId { get; set; }
        public virtual Test Test { get; set; } 
    }
}
