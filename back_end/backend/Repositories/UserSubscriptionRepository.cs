using backend.DataAccess.DAO;
using backend.Models;

namespace backend.Repositories
{
    public class UserSubscriptionRepository : IUserSubscriptionRepository
    {
        private readonly UserSubscriptionDAO _userSubscriptionDAO;

        public UserSubscriptionRepository(UserSubscriptionDAO userSubscriptionDAO)
        {
            _userSubscriptionDAO = userSubscriptionDAO;
        }

        public async Task CreateAsync(UserSubscription subscription)
        {
            await _userSubscriptionDAO.CreateAsync(subscription);
        }

        public async Task<UserSubscription?> GetByUserIdAsync(int userId)
        {
           return await _userSubscriptionDAO.GetByUserIdAsync(userId);
        }

        public async Task UpdateAsync(UserSubscription subscription)
        {
            await _userSubscriptionDAO.UpdateAsync(subscription);
        }
    }
}
