using backend.Models;

namespace backend.Repositories
{
    public interface INotificationRepository
    {
        Task AddNotificationAsync(Notification notification);
        Task<List<Notification>> GetNotificationsByUserIdAsync(int userId);
        Task MarkAsReadAsync(int userId);

    }
}
