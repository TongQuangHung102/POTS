using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace backend.Services
{
    public class TestCategoryService
    {
        private readonly ITestCategoryRepository _testCategoryRepository;

        public TestCategoryService(ITestCategoryRepository testCategoryRepository)
        {
            _testCategoryRepository = testCategoryRepository;
        }

        public async Task<List<TestCategory>> GetAllCategories()
        {
            return await _testCategoryRepository.GetAllAsync();
        }
        public async Task<TestCategory> GetCategoryById(int id)
        {
            return await _testCategoryRepository.GetByIdAsync(id);
        }
        public async Task<IActionResult> AddCategory(TestCategory category)
        {
            try
            {
                await _testCategoryRepository.AddTestCategoryAsync(category);

                return new OkObjectResult(new
                {
                    Message = "Thêm mới thành công!"
                });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { Message = "Lỗi khi thêm mới.", Error = ex.Message })
                {
                    StatusCode = 500
                };
            }

        }
        public async Task<IActionResult> UpdateCategory(int id, TestCategory category)
        {
            try
            {
                var existingCategory = await _testCategoryRepository.GetByIdAsync(id);
                if (existingCategory == null)
                    throw new ArgumentNullException("Không tìm thấy nội dung");

                existingCategory.CategoryName = category.CategoryName;
                existingCategory.IsVisible = category.IsVisible;

                await _testCategoryRepository.UpdateTestCategoryAsync(existingCategory);
                return new OkObjectResult(new { Message = "Cập nhật thành công!" });
            }
            catch (Exception ex)
            {
                return new ObjectResult(new { Message = "Lỗi khi cập nhật grade.", Error = ex.Message })
                {
                    StatusCode = 500
                };
            }

        }

    }
}
