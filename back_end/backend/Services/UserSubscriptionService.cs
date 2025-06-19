using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class UserSubscriptionService : IUserSubscriptionService
    {
        private readonly IUserSubscriptionRepository _userSubscriptionRepository;
        private readonly ISubscriptionPlanRepository _subscriptionPlanRepository;

        public UserSubscriptionService(IUserSubscriptionRepository userSubscriptionRepository, ISubscriptionPlanRepository subscriptionPlanRepository)
        {
            _userSubscriptionRepository = userSubscriptionRepository;
            _subscriptionPlanRepository = subscriptionPlanRepository;
        }

        public async Task AssignFreePlanToUserAsync(int userId)
        {
            var freePlan = await _subscriptionPlanRepository.GetFirstFreePlanAsync();

            if (freePlan == null)
                return;

            var now = DateTime.UtcNow.AddHours(7);

            var subscription = new UserSubscription
            {
                UserId = userId,
                PlanId = freePlan.PlanId,
                StartDate = now,
                EndDate = freePlan.Duration > 0 ? now.AddDays(freePlan.Duration) : now.AddYears(10),
                CreatedAt = now
            };

            await _userSubscriptionRepository.CreateAsync(subscription);
        }

        public async Task CreateOrUpdatePlanForUserAsync(int userId, int planId)
        {
            var plan = await _subscriptionPlanRepository.GetByIdAsync(planId);

            if (plan == null)
                return;

            var now = DateTime.UtcNow.AddHours(7);

            var existingSub = await _userSubscriptionRepository.GetByUserIdAsync(userId);



            if (existingSub == null)
            {
                var newSub = new UserSubscription
                {
                    UserId = userId,
                    PlanId = planId,
                    StartDate = now,
                    EndDate = now.AddDays(plan.Duration),
                    CreatedAt = now
                };
                await _userSubscriptionRepository.CreateAsync(newSub);
            }
            else
            {
                var currentPlan = await _subscriptionPlanRepository.GetByIdAsync(existingSub.PlanId);
                if (existingSub.EndDate > now && currentPlan.Price != 0)
                {
                    // Còn hạn -> cộng dồn thời gian
                    existingSub.EndDate = existingSub.EndDate.AddDays(plan.Duration);
                }
                else
                {
                    // Hết hạn -> cập nhật lại thời gian mới
                    existingSub.StartDate = now;
                    existingSub.EndDate = now.AddDays(plan.Duration);
                }
                existingSub.UpdatedAt = now;
                existingSub.PlanId = planId;
                await _userSubscriptionRepository.UpdateAsync(existingSub);
            }
        }

        public async Task<UserSubscription?> GetByUserIdAsync(int userId)
        {
            return await _userSubscriptionRepository.GetByUserIdAsync(userId);
        }

        public async Task<(bool IsTrial, DateTime? ExpirationDate, int? MaxExercises)> GetSubscriptionInfoAsync(int userId)
        {
            try
            {
                var subscription = await _userSubscriptionRepository.GetByUserIdAsync(userId);

                if (subscription == null || subscription.SubscriptionPlan == null)
                {
                    return (true, null, null);
                }

                if (subscription.SubscriptionPlan.Price == 0)
                {
                    return (true, null, subscription.SubscriptionPlan.MaxExercisesPerDay);
                }

                if (subscription.EndDate < DateTime.UtcNow.AddHours(7))
                {
                    return (true, null, null);
                }

                return (false, subscription.EndDate, null);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Lỗi khi lấy thông tin gói của user {userId}: {ex.Message}");
                return (true, null, null);
            }
        }

    }
}
