using backend.Dtos.Users;
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
    public class UserParentStudentServiceTests
    {
        private readonly Mock<IUserParentStudentRepository> _mockParentStudentRepo;
        private readonly Mock<IUserRepository> _mockUserRepo;
        private readonly Mock<ISendMailService> _mockSendMailService;
        private readonly UserParentStudentService _service;

        public UserParentStudentServiceTests()
        {
            _mockParentStudentRepo = new Mock<IUserParentStudentRepository>();
            _mockUserRepo = new Mock<IUserRepository>();
            _mockSendMailService = new Mock<ISendMailService>();

            _service = new UserParentStudentService(
                _mockParentStudentRepo.Object,
                _mockUserRepo.Object,
                _mockSendMailService.Object
            );
        }

        [Fact]
        public async Task LinkAccount_ShouldThrow_WhenStudentNotFound()
        {

            var input = new LinkAccount { Email = "student@example.com", ParentId = 1 };
            _mockUserRepo.Setup(r => r.GetUserByEmailAsync(input.Email)).ReturnsAsync((User)null);

    
            var ex = await Assert.ThrowsAsync<KeyNotFoundException>(() => _service.LinkAccountChild(input));
            Assert.Equal("Người dùng không tồn tại.", ex.Message);
        }

        [Fact]
        public async Task ResendVerificationCodeAsync_ShouldReturnSuccessMessage_WhenValid()
        {
      
            var email = "student@example.com";
            var parentId = 1;
            var student = new User { UserId = 5 };
            var link = new UserParentStudent { ParentId = parentId, StudentId = 5 };

            _mockUserRepo.Setup(r => r.GetUserByEmailAsync(email)).ReturnsAsync(student);
            _mockParentStudentRepo.Setup(r => r.GetByParentIdAndStudentId(parentId, student.UserId)).ReturnsAsync(link);

            _mockSendMailService.Setup(s => s.SendEmailLinkAccountAsync(email, It.IsAny<string>()))
                .Returns(Task.CompletedTask);

    
            var result = await _service.ResendVerificationCodeAsync(email, parentId);

      
            Assert.Equal("Mã xác nhận mới đã được gửi.", result);
        }

        [Fact]
        public async Task DeleteParentStudentAsync_CallsRepositoryMethod()
        {
     
            int parentId = 1;
            int studentId = 2;

            _mockParentStudentRepo.Setup(r => r.DeleteParentStudentAsync(parentId, studentId))
                .Returns(Task.CompletedTask)
                .Verifiable();

   
            await _service.DeleteParentStudentAsync(parentId, studentId);

  
            _mockParentStudentRepo.Verify();
        }

        [Fact]
        public async Task LinkAccount_ShouldThrow_WhenParentLinksMaxStudents()
        {
      
            var input = new LinkAccount { Email = "student6@example.com", ParentId = 1 };
            _mockUserRepo.Setup(r => r.GetUserByEmailAsync(input.Email)).ReturnsAsync(new User { UserId = 6 });
            _mockParentStudentRepo.Setup(r => r.GetByParentIdAndStudentId(1, 6)).ReturnsAsync((UserParentStudent)null);
            _mockParentStudentRepo.Setup(r => r.CountStudentsByParent(1)).ReturnsAsync(5);

       
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.LinkAccountChild(input));
            Assert.Equal("Phụ huynh đã liên kết tối đa 5 học sinh.", ex.Message);
        }

        [Fact]
        public async Task LinkAccount_ShouldThrow_WhenStudentLinksMaxParents()
        {
        
            var input = new LinkAccount { Email = "existing_student@example.com", ParentId = 3 };
            _mockUserRepo.Setup(r => r.GetUserByEmailAsync(input.Email)).ReturnsAsync(new User { UserId = 7 });
            _mockParentStudentRepo.Setup(r => r.GetByParentIdAndStudentId(3, 7)).ReturnsAsync((UserParentStudent)null);
            _mockParentStudentRepo.Setup(r => r.CountStudentsByParent(3)).ReturnsAsync(2); 
            _mockParentStudentRepo.Setup(r => r.CountParentsByStudent(7)).ReturnsAsync(2);

         
            var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => _service.LinkAccountChild(input));
            Assert.Equal("Học sinh đã liên kết tối đa 2 phụ huynh.", ex.Message);
        }
    }
}
