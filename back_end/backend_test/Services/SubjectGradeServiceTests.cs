using backend.Dtos.SubjectGrade;
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
    public class SubjectGradeServiceTests
    {
        private readonly Mock<ISubjectGradeRepository> _mockRepo;
        private readonly SubjectGradeService _service;

        public SubjectGradeServiceTests()
        {
            _mockRepo = new Mock<ISubjectGradeRepository>();
            _service = new SubjectGradeService(_mockRepo.Object);
        }

        [Fact]
        public async Task GetBySubjectGradeAsync_ShouldReturnSubjectGrade_WhenExists()
        {
           
            int gradeId = 1;
            int subjectId = 101;
            var expectedSubjectGrade = new SubjectGrade { GradeId = gradeId, SubjectId = subjectId };
            _mockRepo.Setup(repo => repo.GetByGradeAndSubjectAsync(gradeId, subjectId)).ReturnsAsync(expectedSubjectGrade);

            var result = await _service.GetBySubjectGradeAsync(gradeId, subjectId);

 
            Assert.Equal(expectedSubjectGrade, result);
        }

        [Fact]
        public async Task GetBySubjectGradeAsync_ShouldThrowInvalidOperationException_WhenNotExists()
        {

            int gradeId = 1;
            int subjectId = 101;
            _mockRepo.Setup(repo => repo.GetByGradeAndSubjectAsync(gradeId, subjectId)).ReturnsAsync((SubjectGrade)null);


            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.GetBySubjectGradeAsync(gradeId, subjectId));
        }


        [Fact]
        public async Task AddNewSubjectGrade_ShouldThrowInvalidOperationException_WhenExists()
        {

            var subjectGradeDto = new SubjectGradeDto { GradeId = 1, SubjectId = 101 };
            _mockRepo.Setup(repo => repo.GetByGradeAndSubjectAsync(subjectGradeDto.GradeId, subjectGradeDto.SubjectId)).ReturnsAsync(new SubjectGrade());

    
            await Assert.ThrowsAsync<InvalidOperationException>(() => _service.AddNewSubjectGrade(subjectGradeDto));
        }
    }
}
