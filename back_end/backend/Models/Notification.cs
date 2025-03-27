using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Notification
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserId { get; set; }  

        [ForeignKey("UserId")] 
        public virtual User User { get; set; } 
        public string Title { get; set; }  
        public string Content { get; set; } 
        public string Type { get; set; }   
        public bool IsRead { get; set; }  
        public DateTime CreatedAt { get; set; } 
        public DateTime? ReadAt { get; set; }
    }
}
