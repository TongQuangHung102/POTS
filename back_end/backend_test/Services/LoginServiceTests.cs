using backend.Dtos.AIQuestions;
using backend.Dtos.Auth;
using backend.Helpers;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Xunit;


namespace backend_test.Services
{
    public class LoginServiceTests
    {
        private readonly Mock<IAuthRepository> _authRepositoryMock;
        private readonly Mock<PasswordEncryption> _passwordEncryptionMock;
        private readonly Mock<IGradeRepository> _gradeRepositoryMock;
        private readonly LoginService _loginService;
        private readonly Mock<IUserSubscriptionService> _userSubscriptionServiceMock;
        public LoginServiceTests()
        {
            _authRepositoryMock = new Mock<IAuthRepository>();
            _passwordEncryptionMock = new Mock<PasswordEncryption>();
            _gradeRepositoryMock = new Mock<IGradeRepository>();
            _loginService = new LoginService(
                _authRepositoryMock.Object,
                _passwordEncryptionMock.Object,
                _gradeRepositoryMock.Object,
                _userSubscriptionServiceMock.Object);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnNull_WhenUserNotFound()
        {
            _authRepositoryMock.Setup(repo => repo.GetUserByEmail(It.IsAny<string>())).ReturnsAsync((User)null);

            var result = await _loginService.LoginAsync(new LoginRequestDto { Email = "test@example.com", Password = "password" });

            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnNull_WhenPasswordIsIncorrect()
        {
            var user = new User { Email = "test@example.com", Password = "hashedPassword" };
            _authRepositoryMock.Setup(repo => repo.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(user);
            _passwordEncryptionMock.Setup(pe => pe.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

            var result = await _loginService.LoginAsync(new LoginRequestDto { Email = "test@example.com", Password = "wrongPassword" });

            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowException_WhenUserIsNotActive()
        {
            var user = new User { Email = "test@example.com", Password = "hashedPassword", IsActive = false };
            _authRepositoryMock.Setup(repo => repo.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(user);
            _passwordEncryptionMock.Setup(pe => pe.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);

            await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _loginService.LoginAsync(new LoginRequestDto { Email = "test@example.com", Password = "password" }));
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenLoginSuccessful()
        {
            var user = new User { UserId = 1, Email = "test@example.com", Password = "hashedPassword", IsActive = true, Role = 1 };
            _authRepositoryMock.Setup(repo => repo.GetUserByEmail(It.IsAny<string>())).ReturnsAsync(user);
            _passwordEncryptionMock.Setup(pe => pe.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
            _authRepositoryMock.Setup(repo => repo.UpdateLastLoginTimeAsync(It.IsAny<User>())).Returns(Task.CompletedTask);

            var result = await _loginService.LoginAsync(new LoginRequestDto { Email = "test@example.com", Password = "password" });

            Assert.NotNull(result);
            Assert.Equal(1, result.UserId);
            Assert.False(string.IsNullOrEmpty(result.Token));
        }
    }
}
