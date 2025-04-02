using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.DataAccess.DAO
{
    public class NotificationDAO
    {
        private readonly MyDbContext _dbContext;

        public NotificationDAO(MyDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            _dbContext.Notifications.Add(notification);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<List<Notification>> GetNotificationsByUserIdAsync(int userId)
        {
            DateTime tenDaysAgo = DateTime.UtcNow.AddDays(-10);
            return await _dbContext.Notifications
                .Where(n => n.UserId == userId && n.CreatedAt >= tenDaysAgo)
                .OrderByDescending(n => n.CreatedAt)
                .ToListAsync();
        }

        public async Task MarkAllAsReadAsync(int userId)
        {
            var unreadNotifications = await _dbContext.Notifications
                .Where(n => n.UserId == userId && !n.IsRead)
                .ToListAsync();

            if (unreadNotifications.Any())
            {
                foreach (var notification in unreadNotifications)
                {
                    notification.IsRead = true;
                }

                await _dbContext.SaveChangesAsync();
            }
        }

    }
}
