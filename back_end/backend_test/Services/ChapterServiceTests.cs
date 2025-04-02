/*using backend.Models;
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
        private readonly Mock<ICurriculumRepository> _mockRepo;

        public ChapterServiceTests()
        {
            _mockRepo = new Mock<ICurriculumRepository>();
            _service = new ChapterService(_mockRepo.Object);
        }

        [Fact]
        public async Task AddChaptersAsync_ShouldAddChapters_WhenValidInput()
        {
            string input = "Chương 1: Toán cơ bản Chương 2: Hình học";
            _mockRepo.Setup(repo => repo.GetAllChapterAsync()).ReturnsAsync(new List<Chapter>());
            _mockRepo.Setup(repo => repo.AddChaptersAsync(It.IsAny<List<Chapter>>())).Returns(Task.CompletedTask);

            await _service.AddChaptersAsync(input);

            _mockRepo.Verify(repo => repo.AddChaptersAsync(It.IsAny<List<Chapter>>()), Times.Once);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task AddChaptersAsync_ShouldThrowException_WhenInputIsEmpty(string input)
        {
            await Assert.ThrowsAsync<ArgumentException>(async () => await _service.AddChaptersAsync(input));
        }

        [Fact]
        public async Task AddChaptersAsync_ShouldThrowException_WhenInputIsMalformed()
        {
            string input = "Chương abc: Toán";

            await Assert.ThrowsAsync<FormatException>(async () => await _service.AddChaptersAsync(input));
        }

        [Fact]
        public async Task AddChaptersAsync_ShouldThrowException_WhenDuplicateChapterExists()
        {
            string input = "Chương 1: Toán cơ bản Chương 2: Hình học";
            var existingChapters = new List<Chapter>
        {
            new Chapter { Order = 1, ChapterName = "Toán cơ bản", IsVisible = true }
        };
            _mockRepo.Setup(repo => repo.GetAllChapterAsync()).ReturnsAsync(existingChapters);

            await Assert.ThrowsAsync<InvalidOperationException>(async () => await _service.AddChaptersAsync(input));
        }

        [Fact]
        public async Task AddChaptersAsync_ShouldThrowException_WhenDuplicateChaptersInInput()
        {
            string input = "Chương 1: Toán cơ bản Chương 1: Hình học";

            await Assert.ThrowsAsync<ArgumentException>(async () => await _service.AddChaptersAsync(input));
        }

        [Fact]
        public async Task AddChaptersAsync_ShouldThrowException_WhenMissingChapterNumber()
        {
            string input = "Toán cơ bản\nHình học";

            await Assert.ThrowsAsync<FormatException>(async () => await _service.AddChaptersAsync(input));
        }
    }
}
*/