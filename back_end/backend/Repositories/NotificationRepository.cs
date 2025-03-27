using backend.DataAccess.DAO;
using backend.Models;

namespace backend.Repositories
{
    public class NotificationRepository : INotificationRepository
    {
        private readonly NotificationDAO _notificationDAO;

        public NotificationRepository(NotificationDAO notificationDAO)
        {
            _notificationDAO = notificationDAO;
        }

        public async Task AddNotificationAsync(Notification notification)
        {
            await _notificationDAO.AddNotificationAsync(notification);
        }

        public async Task<List<Notification>> GetNotificationsByUserIdAsync(int userId)
        {
            return await _notificationDAO.GetNotificationsByUserIdAsync(userId);
        }

        public async Task MarkAsReadAsync(int notificationId)
        {
            await _notificationDAO.MarkAsReadAsync(notificationId);
        }
    }
}
