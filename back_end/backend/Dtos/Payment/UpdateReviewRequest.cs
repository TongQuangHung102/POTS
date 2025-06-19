namespace backend.Dtos.Payment
{
    public class UpdateReviewRequest
    {
        public int PaymentId { get; set; }
        public bool IsReview { get; set; }
    }
}
