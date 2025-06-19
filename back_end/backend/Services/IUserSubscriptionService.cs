using backend.Models;

namespace backend.Services
{
    public interface IUserSubscriptionService
    {
        Task AssignFreePlanToUserAsync(int userId);
        Task CreateOrUpdatePlanForUserAsync(int userId, int planId);
        Task<UserSubscription?> GetByUserIdAsync(int userId);
        Task<(bool IsTrial, DateTime? ExpirationDate, int? MaxExercises)> GetSubscriptionInfoAsync(int userId);
    }
}
