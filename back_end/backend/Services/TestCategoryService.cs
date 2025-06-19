using backend.Models;
using backend.Repositories;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace backend.Services
{
    public class TestCategoryService
    {
        private readonly ITestCategoryRepository _testCategoryRepository;

        public TestCategoryService(ITestCategoryRepository testCategoryRepository)
        {
            _testCategoryRepository = testCategoryRepository;
        }

        public async Task<List<TestCategory>> GetAllCategoriesAsync()
        {
            try
            {
                return await _testCategoryRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi khi lấy danh sách danh mục.", ex);
            }
        }

        public async Task<TestCategory?> GetCategoryByIdAsync(int id)
        {
            if (id <= 0)
                throw new ArgumentException("ID phải lớn hơn 0.", nameof(id));
            try
            {
                return await _testCategoryRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi lấy danh mục có ID: {id}", ex);
            }
        }

        public async Task<bool> AddCategoryAsync(TestCategory category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category), "Tên danh mục bài kiểm tra không được null.");

            if (string.IsNullOrWhiteSpace(category.CategoryName))
                throw new ArgumentNullException(nameof(category.CategoryName), "Tên danh mục không được rỗng hoặc chỉ chứa khoảng trắng.");

            var existingCategories = await _testCategoryRepository.GetAllAsync();
            if (existingCategories.Any(c => c.CategoryName.ToLower() == category.CategoryName.ToLower()))
            {
                throw new InvalidOperationException("Tên loại bài kiểm tra đã tồn tại");
            }

            try
            {
                await _testCategoryRepository.AddTestCategoryAsync(category);
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Lỗi không xác định khi thêm danh mục mới.", ex);
            }
        }



        public async Task<bool> UpdateCategoryAsync(int id, TestCategory category)
        {
            if (category == null)
                throw new ArgumentNullException(nameof(category), "Tên danh mục bài kiểm tra không được null.");

            var existingCategory = await _testCategoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
                throw new KeyNotFoundException($"Không tìm thấy danh mục có ID: {id}");

            var allCategories = await _testCategoryRepository.GetAllAsync();
            var isDuplicateName = allCategories.Any(c => c.TestCategoryId != id && c.CategoryName.ToLower() == category.CategoryName.ToLower());
            if (isDuplicateName)
            {
                throw new InvalidOperationException("Tên loại bài kiểm tra đã tồn tại");
            }

            existingCategory.CategoryName = category.CategoryName;
            existingCategory.IsVisible = category.IsVisible;

            await _testCategoryRepository.UpdateTestCategoryAsync(existingCategory);
            return true;
        }

    }
}