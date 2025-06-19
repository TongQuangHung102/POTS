using backend.Dtos.Subject;
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
    public class SubjectServiceTests
    {
        private readonly Mock<ISubjectRepository> _mockRepo;
        private readonly SubjectService _service;

        public SubjectServiceTests()
        {
            _mockRepo = new Mock<ISubjectRepository>();
            _service = new SubjectService(_mockRepo.Object);
        }

        [Theory]
        [InlineData("Vật lý", true)]   
        [InlineData("Toán", false)]    
        [InlineData("", false)]      
        [InlineData(null, false)]   
        public async Task AddNewSubjectAsync_ShouldHandleDuplicateNames(string subjectName, bool shouldSucceed)
        {
            var existingSubject = subjectName == "Toán" ? new Subject { SubjectName = "Toán" } : null;

            _mockRepo.Setup(repo => repo.GetSubjectByName(subjectName)).ReturnsAsync(existingSubject);

            if (shouldSucceed)
            {
                await _service.AddNewSubjectAsync(new SubjectDto { SubjectName = subjectName });
                _mockRepo.Verify(repo => repo.AddSubjectAsync(It.IsAny<Subject>()), Times.Once);
            }
            else
            {
                await Assert.ThrowsAsync<ArgumentException>(() => _service.AddNewSubjectAsync(new SubjectDto { SubjectName = subjectName }));
            }
        }

        [Theory]
        [InlineData(1, true, true)]  
        [InlineData(0, true, false)]  
        [InlineData(-1, true, false)] 
        public async Task EditSubjectAsync_ShouldHandleSubjectUpdate(int subjectId, bool isVisible, bool shouldSucceed)
        {
            var existingSubject = subjectId > 0 && subjectId < 10
                ? new Subject { SubjectName = "Vật lý", SubjectId = subjectId, IsVisible = !isVisible }
                : null;

            _mockRepo.Setup(repo => repo.GetSubjectByIdAsync(subjectId)).ReturnsAsync(existingSubject);
            _mockRepo.Setup(repo => repo.UpdateSubjectAsync(It.IsAny<Subject>())).Returns(Task.CompletedTask);

            if (shouldSucceed)
            {
                await _service.EditSubjectAsync(new SubjectEditDto { SubjectId = subjectId, IsVisible = isVisible });
                _mockRepo.Verify(repo => repo.UpdateSubjectAsync(It.IsAny<Subject>()), Times.Once);
            }
            else
            {
                await Assert.ThrowsAsync<ArgumentException>(() => _service.EditSubjectAsync(new SubjectEditDto { SubjectId = subjectId, IsVisible = isVisible }));
            }
        }
    }
}
