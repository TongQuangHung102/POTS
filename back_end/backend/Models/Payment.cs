using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace backend.Models
{
    public class Payment
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
        public decimal ReceivedAmount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PaymentStatus { get; set; }
        public string? FailureReason { get; set; }
        public bool? IsManuallyReviewed { get; set; } 
        public string? ManualReviewerNote { get; set; }
        public DateTime? ReviewedAt { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
        public int PlanId { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public int? StudentId { get; set; }
        [ForeignKey("StudentId")]
        public virtual User Student { get; set; }

        [ForeignKey("PlanId")]
        public virtual SubscriptionPlan Plan { get; set; }

    }
}
