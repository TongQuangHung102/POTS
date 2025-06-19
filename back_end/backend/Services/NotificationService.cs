using backend.DataAccess.DAO;
using backend.Dtos.Dashboard;
using backend.Hubs;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.SignalR;

namespace backend.Services
{
    public class NotificationService
    {
        private readonly IUserRepository _userRepository;
        private readonly INotificationRepository _notificationRepository;
        private readonly IUserParentStudentRepository _userParentStudentRepository;
        private readonly IHubContext<NotificationHub> _hubContext;
        private readonly ISendMailService _sendMailService;

        public NotificationService(IUserRepository userRepository, INotificationRepository notificationRepository, IUserParentStudentRepository userParentStudentRepository, IHubContext<NotificationHub> hubContext, ISendMailService sendMailService)
        {
            _userRepository = userRepository;
            _notificationRepository = notificationRepository;
            _userParentStudentRepository = userParentStudentRepository;
            _hubContext = hubContext;
            _sendMailService = sendMailService;
        }

        public async Task SendNotificationsToInactiveStudentsAsync()
        {
            DateTime threeDaysAgo = DateTime.UtcNow.AddHours(7).AddDays(-3);
            var students = await _userRepository.GetInactiveStudentsFor3DaysAsync(threeDaysAgo);

            foreach (var student in students)
            {
                var parents = await _userParentStudentRepository.GetParentsByStudentIdAsync(student.UserId);

                foreach (var parent in parents)
                {
                    var noti = new Notification
                    {
                        UserId = parent.UserId,
                        Title = "Đã lâu rồi con bạn chưa học",
                        Content = $"Con ({student.UserName}) của bạn đã 3 ngày không học. Hãy khuyến khích bé học nhé!",
                        Type = "Nhắc nhở",
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow.AddHours(7)
                    };

                    await _notificationRepository.AddNotificationAsync(noti);

                    await _hubContext.Clients.User(parent.UserId.ToString()).SendAsync("ReceiveNotification");

                    await _sendMailService.SendEmailNotificationAsync(parent.Email, noti.Title, noti.Content);
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

        public async Task MarkAllAsRead(int userId)
        {
            await _notificationRepository.MarkAsReadAsync(userId);
        }
    }
}
