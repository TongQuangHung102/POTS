using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Report
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ReportId { get; set; }
        public int QuestionId { get; set; } 
        public int UserId { get; set; }  
        public string Reason { get; set; }  
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow; 
        public string Status { get; set; } 

        [ForeignKey("QuestionId")]
        public virtual Question Question { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
