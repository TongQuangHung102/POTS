using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.DataAccess.DAO
{
    public class UserSubscriptionDAO
    {
        private readonly MyDbContext _context;

        public UserSubscriptionDAO(MyDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(UserSubscription subscription)
        {
            await _context.UserSubscriptions.AddAsync(subscription);
            await _context.SaveChangesAsync();
        }

        public async Task<UserSubscription?> GetByUserIdAsync(int userId)
        {
            return await _context.UserSubscriptions
                .Include(us => us.User)
                .Include(us => us.SubscriptionPlan)
                .FirstOrDefaultAsync(us => us.UserId == userId);
        }

        public async Task UpdateAsync(UserSubscription subscription)
        {
            _context.UserSubscriptions.Update(subscription);
            await _context.SaveChangesAsync();
        }
    }
}
