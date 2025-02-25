using backend.Models;

namespace backend.Repositories
{
    public interface ISubscriptionPlanRepository
    {
        Task AddSubscriptionPlanAsync(SubscriptionPlan plan);
        Task<List<SubscriptionPlan>> GetAllAsync();
        Task<SubscriptionPlan> GetByIdAsync(int id);
        Task UpdateAsync(SubscriptionPlan plan);
        Task<SubscriptionPlan> GetByNameAsync(string planName);
    }
}
