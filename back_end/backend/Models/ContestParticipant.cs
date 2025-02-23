using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class ContestParticipant
    {
        [Key]
        public int ContestParticipantId { get; set; }
        public int ContestId { get; set; }
        public int UserId { get; set; }
        public DateTime Register_At { get; set; }

        [ForeignKey("ContestId")]
        public virtual Contest Contest { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
