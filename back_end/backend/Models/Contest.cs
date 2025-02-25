using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Contest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ContestId { get; set; }
        public string ContestName { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; }
        public bool IsVisible { get; set; }
        public string Description { get; set; }
        public DateTime Create_At { get; set; }
        public virtual ICollection<ContestParticipant> ContestParticipants { get; set; }
        public virtual ICollection<ContestQuestion> ContestQuestions { get; set; }
    }
}
