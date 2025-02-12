namespace backend.Dtos
{
    public class SubscriptionPlanDto
    {
        public int PlanId { get; set; }
        public string PlanName { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public int MaxExercisesPerDay { get; set; }
        public bool IsAIAnalysis { get; set; }
        public bool IsPersonalization { get; set; }
        public bool IsBasicStatistics { get; set; }
        public bool IsAdvancedStatistics { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
}
