namespace backend.Dtos.Payment
{
    public class PaymentCreateDto
    {
        public int PlanId { get; set; }
        public int StudentId { get; set; }
        public int UserId { get; set; }
        public decimal Amount { get; set; }
    }
}
