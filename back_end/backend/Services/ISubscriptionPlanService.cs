using backend.Dtos.Subscriptions;
using backend.Models;

namespace backend.Services
{
    public interface ISubscriptionPlanService
    {
        Task AddSubscriptionPlanAsync(SubscriptionPlan plan);
        Task<List<SubscriptionPlanDto>> GetAllPlansAsync();
        Task<bool> UpdatePlanAsync(int id, SubscriptionPlanDto dto);
        Task<SubscriptionPlanDto> GetPlanDetailAsync(int id);
    }
}
