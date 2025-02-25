using backend.Dtos;
using backend.Helpers;
using backend.Models;
using backend.Repositories;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace backend_test.Services
{
    public class RegisterServiceTests
    {
        private readonly Mock<IAuthRepository> _authRepositoryMock;
        private readonly Mock<SendMailService> _sendMailServiceMock;
        private readonly Mock<PasswordEncryption> _passwordEncryptionMock;
        private readonly RegisterService _registerService;

        public RegisterServiceTests()
        {
            _authRepositoryMock = new Mock<IAuthRepository>();
            _sendMailServiceMock = new Mock<SendMailService>();
            _passwordEncryptionMock = new Mock<PasswordEncryption>();

            _registerService = new RegisterService(
                _authRepositoryMock.Object,
                _sendMailServiceMock.Object,
                _passwordEncryptionMock.Object);
        }
        [Fact]
        public async Task Register_ShouldReturnBadRequest_WhenEmailAlreadyExists()
        {
            var existingUser = new User { Email = "test@example.com" };
            _authRepositoryMock.Setup(repo => repo.GetUserByEmail(It.IsAny<string>()))
                               .ReturnsAsync(existingUser);

            var request = new RegisterDto { Email = "test@example.com", Password = "password123", UserName = "Test", Role = 1 };

            var result = await _registerService.Register(request);

            var badRequestResult = Assert.IsType<BadRequestObjectResult>(result);
            Assert.Equal(400, badRequestResult.StatusCode);
        }

        [Fact]
        public async Task Register_ShouldCreateUser_WhenEmailNotExists()
        {
            _authRepositoryMock.Setup(repo => repo.GetUserByEmail(It.IsAny<string>()))
                               .ReturnsAsync((User)null);
            _passwordEncryptionMock.Setup(pe => pe.HashPassword(It.IsAny<string>()))
                                   .Returns("hashedPassword");

            var request = new RegisterDto { Email = "new@example.com", Password = "password123", UserName = "Test", Role = 1 };

            var result = await _registerService.Register(request);

            var okResult = Assert.IsType<OkObjectResult>(result);
            Assert.Equal(200, okResult.StatusCode);
        }
    }
}
