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
    public class TestCategoryServiceTests
    {
        private readonly TestCategoryService _service;
        private readonly Mock<ITestCategoryRepository> _mockRepo;

        public TestCategoryServiceTests()
        {
            _mockRepo = new Mock<ITestCategoryRepository>();
            _service = new TestCategoryService(_mockRepo.Object);
        }

        [Theory]
        [InlineData("Bài kiểm tra cuối kỳ II", true, true)] 
        [InlineData("", true, false)] 
        [InlineData(null, false, false)] 
        public async Task AddCategoryAsync_ShouldHandleCategoryCorrectly(string categoryName, bool isVisible, bool shouldSucceed)
        {
            var category = new TestCategory { CategoryName = categoryName, IsVisible = isVisible };
            _mockRepo.Setup(repo => repo.GetAllAsync()).ReturnsAsync(new List<TestCategory>());
            _mockRepo.Setup(repo => repo.AddTestCategoryAsync(It.IsAny<TestCategory>())).Returns(Task.CompletedTask);

            if (shouldSucceed)
            {
                var result = await _service.AddCategoryAsync(category);
                Assert.True(result);
                _mockRepo.Verify(repo => repo.AddTestCategoryAsync(It.IsAny<TestCategory>()), Times.Once);
            }
            else
            {
                await Assert.ThrowsAsync<ArgumentNullException>(async () => await _service.AddCategoryAsync(category));
            }
        }

        [Theory]
        [InlineData(1, "Bài kiểm tra cuối năm", false, true)]  
        [InlineData(0, "Bài kiểm tra mới", true, false)] 
        [InlineData(-1, "Bài kiểm tra mới", true, false)] 
        public async Task UpdateCategoryAsync_ShouldHandleCategoryUpdateCorrectly(int categoryId, string newName, bool isVisible, bool shouldSucceed)
        {
            var existingCategory = categoryId > 0 ? new TestCategory { TestCategoryId = categoryId, CategoryName = "Cũ", IsVisible = true } : null;
            var updatedCategory = new TestCategory { CategoryName = newName, IsVisible = isVisible };

            _mockRepo.Setup(repo => repo.GetByIdAsync(categoryId)).ReturnsAsync(existingCategory);
            _mockRepo.Setup(repo => repo.UpdateTestCategoryAsync(It.IsAny<TestCategory>())).Returns(Task.CompletedTask);

            if (shouldSucceed)
            {
                var result = await _service.UpdateCategoryAsync(categoryId, updatedCategory);
                Assert.True(result);
                _mockRepo.Verify(repo => repo.UpdateTestCategoryAsync(It.IsAny<TestCategory>()), Times.Once);
            }
            else
            {
                await Assert.ThrowsAsync<ArgumentException>(async () => await _service.UpdateCategoryAsync(categoryId, updatedCategory));
            }
        }


    }
}
