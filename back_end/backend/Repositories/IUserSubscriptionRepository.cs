using backend.Models;

namespace backend.Repositories
{
    public interface IUserSubscriptionRepository
    {
        Task CreateAsync(UserSubscription subscription);
        Task<UserSubscription?> GetByUserIdAsync(int userId);
        Task UpdateAsync(UserSubscription subscription);
    }
}
