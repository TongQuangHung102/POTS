/*using backend.Dtos;
using backend.Helpers;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace backend_test.Services
{
    public class LoginServiceTests
    {
        private readonly Mock<IAuthRepository> _authRepositoryMock;
        private readonly Mock<PasswordEncryption> _passwordEncryptionMock;
        private readonly LoginService _loginService;

        public LoginServiceTests()
        {
            _authRepositoryMock = new Mock<IAuthRepository>();
            _passwordEncryptionMock = new Mock<PasswordEncryption>();
            _loginService = new LoginService(_authRepositoryMock.Object, _passwordEncryptionMock.Object);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnNull_WhenUserNotFound()
        {
            _authRepositoryMock.Setup(repo => repo.GetUserByEmail(It.IsAny<string>()))
                               .ReturnsAsync((User)null);

            var request = new LoginRequestDto { Email = "test@example.com", Password = "password123" };
            var result = await _loginService.LoginAsync(request);

            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnNull_WhenPasswordIsNull()
        {
            var user = new User { Email = "test@example.com", Password = null };
            _authRepositoryMock.Setup(repo => repo.GetUserByEmail(It.IsAny<string>()))
                               .ReturnsAsync(user);

            var request = new LoginRequestDto { Email = "test@example.com", Password = "password123" };

            var result = await _loginService.LoginAsync(request);
            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnNull_WhenPasswordIsIncorrect()
        {
            var user = new User { Email = "test@example.com", Password = "hashedPassword" };
            _authRepositoryMock.Setup(repo => repo.GetUserByEmail(It.IsAny<string>()))
                               .ReturnsAsync(user);

            _passwordEncryptionMock.Setup(pe => pe.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                                   .Returns(false);

            var request = new LoginRequestDto { Email = "test@example.com", Password = "wrongPassword" };

            var result = await _loginService.LoginAsync(request);

            Assert.Null(result);
        }

        [Fact]
        public async Task LoginAsync_ShouldThrowUnauthorizedAccessException_WhenUserIsNotActive()
        {
            var user = new User
            {
                UserId = 1,
                Email = "test@example.com",
                Password = "hashedPassword", 
                IsActive = false,
                Role = 1
            };

            _authRepositoryMock.Setup(repo => repo.GetUserByEmail(It.IsAny<string>()))
                               .ReturnsAsync(user);

            _passwordEncryptionMock.Setup(pe => pe.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                                   .Returns(true); 

            var request = new LoginRequestDto { Email = "test@example.com", Password = "password123" };

            var exception = await Assert.ThrowsAsync<UnauthorizedAccessException>(
                () => _loginService.LoginAsync(request)
            );

            Assert.Equal("Tài khoản chưa được kích hoạt", exception.Message);
        }

        [Fact]
        public async Task LoginAsync_ShouldReturnToken_WhenLoginSuccessful()
        {
            var user = new User
            {
                UserId = 1,
                Email = "test@example.com",
                Password = "hashedPassword",
                IsActive = true,
                Role = 1
            };

            _authRepositoryMock.Setup(repo => repo.GetUserByEmail(It.IsAny<string>()))
                               .ReturnsAsync(user);

            _passwordEncryptionMock.Setup(pe => pe.VerifyPassword(It.IsAny<string>(), It.IsAny<string>()))
                                   .Returns(true);

            _authRepositoryMock.Setup(repo => repo.UpdateLastLoginTimeAsync(It.IsAny<User>()))
                               .Returns(Task.CompletedTask);

            var request = new LoginRequestDto { Email = "test@example.com", Password = "password123" };

            var result = await _loginService.LoginAsync(request);

            Assert.NotNull(result);
            Assert.Equal(user.UserId, result.UserId);
            Assert.Equal(user.Role, result.Role);
            Assert.False(string.IsNullOrEmpty(result.Token));
        }


    }
}
*/