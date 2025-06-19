using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using backend.Models;
using backend.Repositories;
using backend.Services;
using backend.Hubs;
using Microsoft.AspNetCore.SignalR;
using Moq;
using Xunit;

namespace backend_test.Services
{
    public class NotificationServiceTests
    {
        [Fact]
        public async Task SendNotificationsToInactiveStudentsAsync_ShouldDoNothing_WhenNoInactiveStudents()
        {
            // Arrange
            var mockUserRepo = new Mock<IUserRepository>();
            var mockNotiRepo = new Mock<INotificationRepository>();
            var mockRelationRepo = new Mock<IUserParentStudentRepository>();
            var mockHubContext = new Mock<IHubContext<NotificationHub>>();
            var mockMailService = new Mock<ISendMailService>();

            mockUserRepo.Setup(r => r.GetInactiveStudentsFor3DaysAsync(It.IsAny<DateTime>()))
                        .ReturnsAsync(new List<User>());

            var service = new NotificationService(mockUserRepo.Object, mockNotiRepo.Object, mockRelationRepo.Object, mockHubContext.Object, mockMailService.Object);

            // Act
            await service.SendNotificationsToInactiveStudentsAsync();

            // Assert
            mockNotiRepo.Verify(n => n.AddNotificationAsync(It.IsAny<Notification>()), Times.Never);
            mockMailService.Verify(m => m.SendEmailNotificationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task SendNotificationsToInactiveStudentsAsync_ShouldNotSend_WhenNoParentFound()
        {
            // Arrange
            var mockUserRepo = new Mock<IUserRepository>();
            var mockNotiRepo = new Mock<INotificationRepository>();
            var mockRelationRepo = new Mock<IUserParentStudentRepository>();
            var mockHubContext = new Mock<IHubContext<NotificationHub>>();
            var mockMailService = new Mock<ISendMailService>();

            var student = new User { UserId = 1, UserName = "StudentA" };

            mockUserRepo.Setup(r => r.GetInactiveStudentsFor3DaysAsync(It.IsAny<DateTime>()))
                        .ReturnsAsync(new List<User> { student });

            mockRelationRepo.Setup(r => r.GetParentByStudentIdAsync(student.UserId))
                            .ReturnsAsync((User)null);

            var service = new NotificationService(mockUserRepo.Object, mockNotiRepo.Object, mockRelationRepo.Object, mockHubContext.Object, mockMailService.Object);

            // Act
            await service.SendNotificationsToInactiveStudentsAsync();

            // Assert
            mockNotiRepo.Verify(n => n.AddNotificationAsync(It.IsAny<Notification>()), Times.Never);
            mockMailService.Verify(m => m.SendEmailNotificationAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task GetNotificationsByUserId_ShouldReturnNotificationsList()
        {
            // Arrange
            var mockUserRepo = new Mock<IUserRepository>();
            var mockNotiRepo = new Mock<INotificationRepository>();
            var mockRelationRepo = new Mock<IUserParentStudentRepository>();
            var mockHubContext = new Mock<IHubContext<NotificationHub>>();
            var mockMailService = new Mock<ISendMailService>();

            int userId = 5;
            var expectedList = new List<Notification>
            {
                new Notification { Id = 1, UserId = userId, Title = "Test", Content = "Test Content" }
            };

            mockNotiRepo.Setup(n => n.GetNotificationsByUserIdAsync(userId))
                        .ReturnsAsync(expectedList);

            var service = new NotificationService(mockUserRepo.Object, mockNotiRepo.Object, mockRelationRepo.Object, mockHubContext.Object, mockMailService.Object);

            // Act
            var result = await service.GetNotificationsByUserId(userId);

            // Assert
            Assert.NotNull(result);
            Assert.Single(result);
            Assert.Equal("Test", result[0].Title);
        }
    }

}
