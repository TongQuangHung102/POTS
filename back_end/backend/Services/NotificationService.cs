using backend.DataAccess.DAO;
using backend.Dtos.Dashboard;
using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class NotificationService
    {
        private readonly IUserRepository _userRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserParentStudentRepository _userParentStudentRepository;

        public NotificationService(IUserRepository userRepository, INotificationRepository notificationRepository, IUserParentStudentRepository userParentStudentRepository)
        {
            _userRepository = userRepository;
            _notificationRepository = notificationRepository;
            _userParentStudentRepository = userParentStudentRepository;
        }

        public async Task SendNotificationsToInactiveStudentsAsync()
        {
            DateTime threeDaysAgo = DateTime.UtcNow.AddDays(-3);
            var students = await _userRepository.GetInactiveStudentsFor3DaysAsync(threeDaysAgo);

            foreach (var student in students)
            {
                var parentId = await _userParentStudentRepository.GetParentIdByStudentIdAsync(student.UserId);
                if(parentId != null && parentId != 0) {
                    var noti = new Notification
                    {
                        UserId = parentId.Value,
                        Title = "Đã lâu rồi con bạn chưa học",
                        Content = $"Con ({student.UserName}) của bạn đã 3 ngày không học. Hãy khuyến khích bé học nhé!",
                        Type = "Nhắc nhở",
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    };

                    await _notificationRepository.AddNotificationAsync(noti);
                }
            }
        }

        public async Task<List<Notification>> GetNotificationsByUserId(int userId)
        {
            try
            {
                return await _notificationRepository.GetNotificationsByUserIdAsync(userId);
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách thông báo.", ex);
            }
        }
    }
}
