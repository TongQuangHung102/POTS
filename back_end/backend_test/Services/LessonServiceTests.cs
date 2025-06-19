using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Moq;
using Xunit;
using backend.Dtos.Curriculum;
using backend.Models;
using backend.Repositories;
using backend.Services;
using System.Text.RegularExpressions;

namespace backend_test.Services
{
    public class LessonServiceTests
    {
        private readonly Mock<ICurriculumRepository> _mockRepo;
        private readonly LessonService _lessonService;

        public LessonServiceTests()
        {
            _mockRepo = new Mock<ICurriculumRepository>();
            _lessonService = new LessonService(_mockRepo.Object);
        }

        [Theory]
        [InlineData("Bài 1: Giới thiệu", 1, "Giới thiệu")]
        [InlineData("Bài 2: Học về C#\nBài 3: OOP trong C#", 2, "Học về C#")]
        public void ParseLessons_ValidInput_ShouldReturnCorrectLessons(string input, int expectedOrder, string expectedName)
        {
            var result = LessonService.ParseLessons(input, 1);

            Assert.NotEmpty(result);
            Assert.Equal(expectedOrder, result[0].Order);
            Assert.Equal(expectedName, result[0].LessonName);
        }

        [Theory]
        [InlineData("Bài 1: ")]
        [InlineData("Bài a: Test")]
        [InlineData("Bài 1: Test\nBài 1: Trùng")]
        public void ParseLessons_InvalidFormat_ShouldThrowException(string input)
        {
            Assert.ThrowsAny<Exception>(() => LessonService.ParseLessons(input, 1));
        }

        [Fact]
        public async Task ValidateDuplicateLessonsAsync_DuplicateLessons_ShouldThrowException()
        {
            var chapterId = 1;
            var existingLessons = new List<Lesson> { new Lesson { Order = 1, LessonName = "Bài 1" } };
            var newLessons = new List<Lesson> { new Lesson { Order = 1, LessonName = "Bài 1" } };

            _mockRepo.Setup(repo => repo.GetLessonByChapterIdAsync(chapterId)).ReturnsAsync(existingLessons);

            var exception = await Assert.ThrowsAsync<InvalidOperationException>(() => _lessonService.ValidateDuplicateLessonsAsync(chapterId, newLessons));
            Assert.Contains("Bài đã tồn tại", exception.Message);
        }
    }
}
