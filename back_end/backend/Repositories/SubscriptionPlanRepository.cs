using backend.DataAccess.DAO;
using backend.Models;

namespace backend.Repositories
{
    public class SubscriptionPlanRepository : ISubscriptionPlanRepository
    {
        private readonly SubscriptionPlanDAO _spDAO;

        public SubscriptionPlanRepository(SubscriptionPlanDAO spDAO)
        {
            _spDAO = spDAO;
        }

        public async Task AddSubscriptionPlanAsync(SubscriptionPlan plan)
        {
            await _spDAO.AddSubscriptionPlanAsync(plan);
        }

        public async Task<List<SubscriptionPlan>> GetAllAsync()
        {
            return await _spDAO.GetAllAsync();
        }

        public async Task<SubscriptionPlan> GetByIdAsync(int id)
        {
            return await _spDAO.GetByIdAsync(id);
        }

        public async Task<SubscriptionPlan> GetByNameAsync(string planName)
        {
            return await _spDAO.GetByNameAsync(planName);
        }

        public async Task UpdateAsync(SubscriptionPlan plan)
        {
            await _spDAO.UpdateAsync(plan);
        }
    }
}
