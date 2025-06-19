namespace backend.Dtos.Payment
{
    public class PaymentResponseDto
    {
        public int PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Bank { get; set; }
        public string Account { get; set; }
        public string Status { get; set; }
        public string QrUrl { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
