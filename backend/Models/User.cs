using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace backend.Models
{
    public class User
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string? Password { get; set; }
        public int? Role { get; set; }
        public DateTime CreateAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public string? GoogleId { get; set; }
        public bool IsActive { get; set; }
        public string? EmailVerificationToken { get; set; }
        public DateTime? TokenExpiry { get; set; }

        [ForeignKey("Role")]
        public virtual Role RoleNavigation { get; set; }
        public virtual ICollection<Chapter> Chapters { get; set; }
        public virtual ICollection<QuizAttempt> QuizAttempts { get; set; }
        public virtual ICollection<StudentPerformance> StudentPerformances { get; set; }
        public virtual ICollection<StudentProgress> StudentProgresses { get; set; }
        public virtual ICollection<UserSubscription> UserSubscriptions { get; set; }
        public virtual ICollection<ContestParticipant> ContestParticipants { get; set; }
        public virtual ICollection<CompetitionResult> CompetitionResults { get; set; }
        public virtual ICollection<StudentTest> StudentTests { get; set; }
        public virtual ICollection<Payment> Payments { get; set; }
    }
}
