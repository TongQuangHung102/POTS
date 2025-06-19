
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
    public class ChapterServiceTests
    {
        private readonly ChapterService _service;
        private readonly Mock<ICurriculumRepository> _mockCurriculumRepo;
        private readonly Mock<IStudentPerformanceRepository> _mockStudentPerformanceRepo;
        private readonly Mock<ISubjectGradeRepository> _mockSubjectGradeRepo;

        public ChapterServiceTests()
        {
            _mockCurriculumRepo = new Mock<ICurriculumRepository>();
            _mockStudentPerformanceRepo = new Mock<IStudentPerformanceRepository>();
            _mockSubjectGradeRepo = new Mock<ISubjectGradeRepository>();
            _service = new ChapterService(_mockCurriculumRepo.Object, _mockStudentPerformanceRepo.Object, _mockSubjectGradeRepo.Object);
        }

        [Fact]
        public async Task AddChaptersAsync_ShouldAddChapters_WhenValidInput()
        {
            string input = "Chương 1: Toán cơ bản Chương 2: Hình học";
            int subjectgradeId = 1;  // Thêm subjectgradeId
            int semester = 1;  // Thêm semester

            _mockCurriculumRepo.Setup(repo => repo.GetAllChapterAsync(subjectgradeId)).ReturnsAsync(new List<Chapter>());
            _mockCurriculumRepo.Setup(repo => repo.AddChaptersAsync(It.IsAny<List<Chapter>>())).Returns(Task.CompletedTask);

            await _service.AddChaptersAsync(subjectgradeId, semester, input);

            _mockCurriculumRepo.Verify(repo => repo.AddChaptersAsync(It.IsAny<List<Chapter>>()), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task AddChaptersAsync_ShouldThrowException_WhenInputIsEmpty(string input)
        {
            int subjectgradeId = 1;
            int semester = 1;

            await Assert.ThrowsAsync<ArgumentException>(async () => await _service.AddChaptersAsync(subjectgradeId, semester, input));
        }

        [Fact]
        public async Task AddChaptersAsync_ShouldThrowException_WhenInputIsMalformed()
        {
            string input = "Chương abc: Toán";
            int subjectgradeId = 1;
            int semester = 1;

            await Assert.ThrowsAsync<FormatException>(async () => await _service.AddChaptersAsync(subjectgradeId, semester, input));
        }

        [Fact]
        public async Task AddChaptersAsync_ShouldThrowException_WhenDuplicateChapterExists()
        {
            string input = "Chương 1: Toán cơ bản Chương 2: Hình học";
            int subjectgradeId = 1;
            int semester = 1;

            var existingChapters = new List<Chapter>
            {
                new Chapter { Order = 1, ChapterName = "Toán cơ bản", IsVisible = true, SubjectGradeId = subjectgradeId }
            };
            _mockCurriculumRepo.Setup(repo => repo.GetAllChapterAsync(subjectgradeId)).ReturnsAsync(existingChapters);

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.AddChaptersAsync(subjectgradeId, semester, input));
        }

        [Fact]
        public async Task AddChaptersAsync_ShouldThrowException_WhenDuplicateChaptersInInput()
        {
            string input = "Chương 1: Toán cơ bản Chương 1: Hình học";
            int subjectgradeId = 1;
            int semester = 1;

            await Assert.ThrowsAsync<ArgumentException>(async () => await _service.AddChaptersAsync(subjectgradeId, semester, input));
        }

        [Fact]
        public async Task AddChaptersAsync_ShouldThrowException_WhenMissingChapterNumber()
        {
            string input = "Toán cơ bản\nHình học";
            int subjectgradeId = 1;
            int semester = 1;

            await Assert.ThrowsAsync<FormatException>(async () => await _service.AddChaptersAsync(subjectgradeId, semester, input));
        }
    }
}
