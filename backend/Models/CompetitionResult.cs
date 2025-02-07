using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Models
{
    public class CompetitionResult
    {
        public int UserId { get; set; }
        public int ContestId { get; set; }
        public int Score { get; set; }
        public TimeSpan TimeTaken { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }

        [ForeignKey("ContestId")]
        public virtual Contest Contest { get; set; }
    }
}
