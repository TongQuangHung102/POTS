using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.DataAccess.DAO
{
    public class SubscriptionPlanDAO
    {
        private readonly MyDbContext _context;
        public SubscriptionPlanDAO(MyDbContext context)
        {
            _context = context;
        }
        public async Task AddSubscriptionPlanAsync(SubscriptionPlan plan)
        {
            plan.CreatedAt = DateTime.UtcNow;
            await _context.SubscriptionPlans.AddAsync(plan);
            await _context.SaveChangesAsync();
        }
        public async Task<List<SubscriptionPlan>> GetAllAsync()
        {
            return await _context.SubscriptionPlans.ToListAsync();
        }
        public async Task<SubscriptionPlan> GetByIdAsync(int id)
        {
            return await _context.SubscriptionPlans.FindAsync(id);
        }

        public async Task UpdateAsync(SubscriptionPlan plan)
        {
            _context.SubscriptionPlans.Update(plan);
            await _context.SaveChangesAsync();
        }

        public async Task<SubscriptionPlan> GetByNameAsync(string planName)
        {
            return await _context.SubscriptionPlans
                .FirstOrDefaultAsync(p => p.PlanName == planName);

        }
    }
}
