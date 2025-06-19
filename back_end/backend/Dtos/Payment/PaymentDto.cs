using backend.Models;
using System.ComponentModel.DataAnnotations.Schema;

namespace backend.Dtos.Payment
{
    public class PaymentDto
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string TransactionId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string PaymentStatus { get; set; }
        public string? FailureReason { get; set; }
        public bool? IsManuallyReviewed { get; set; }
        public string? ManualReviewerNote { get; set; }
        public DateTime? ReviewedAt { get; set; }
        public string PlanName { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public string UserEmail { get; set; }
        public string? StudentEmail { get; set; }
        public decimal ReceivedAmount { get; set; }
    }

    public class PaymentPageDto
    {
        public int TotalPayment { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
        public List<PaymentDto> Data { get; set;}
    }
}
