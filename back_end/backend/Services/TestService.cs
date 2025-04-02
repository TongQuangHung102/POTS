using backend.DataAccess.DAO;
using backend.Dtos.PracticeAndTest;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace backend.Services
{
    public class TestService
    {
        private readonly ITestRepository _testRepository;

        public TestService(ITestRepository testRepository)
        {
            _testRepository = testRepository;
        }

        public async Task<List<TestDto>> GetAllTests()
        {
            var tests = await _testRepository.GetAllAsync();
            return tests.Select(test => new TestDto
            {
                TestId = test.TestId,
                TestName = test.TestName,
                Description = test.Description,
                DurationInMinutes = test.DurationInMinutes,
                MaxScore = test.MaxScore,
                IsVisible = test.IsVisible,
                Order = test.Order  
            }).ToList();
        }

        public async Task<TestDto> GetTestById(int id)
        {
            var test = await _testRepository.GetByIdAsync(id);
            if (test == null) return null;
            return new TestDto
            {
                TestId = test.TestId,
                TestName = test.TestName,
                Description = test.Description,
                DurationInMinutes = test.DurationInMinutes,
                MaxScore = test.MaxScore,
                IsVisible = test.IsVisible,
                Order = test.Order
            };
        }

        public async Task AddTest(TestDto testDto)
        {
            var test = new Test
            {
                TestName = testDto.TestName,
                Description = testDto.Description,
                DurationInMinutes = testDto.DurationInMinutes,
                MaxScore = testDto.MaxScore,
                IsVisible = testDto.IsVisible,
                Order = testDto.Order,
                CreatedAt = testDto.CreatedAt,
                SubjectGradeId = testDto.SubjectGradeId
            };

            await _testRepository.AddAsync(test);
        }

        public async Task UpdateTest(int id, TestDto testDto)
        {
            var existingTest = await _testRepository.GetByIdAsync(id);
            if (existingTest == null)
                throw new KeyNotFoundException("Không tìm thấy bài kiểm tra.");

            existingTest.TestName = testDto.TestName;
            existingTest.Description = testDto.Description;
            existingTest.DurationInMinutes = testDto.DurationInMinutes;
            existingTest.MaxScore = testDto.MaxScore;
            existingTest.IsVisible = testDto.IsVisible;
            existingTest.Order = testDto.Order;
            existingTest.SubjectGradeId = testDto.SubjectGradeId;

            await _testRepository.UpdateAsync(existingTest);
        }
        public async Task<List<TestDto>> GetTestsBySubjectGradeId(int id)
        {
            var tests = await _testRepository.GetTestBySubjectGradeIdAsync(id);
            return tests.Select(test => new TestDto
            {
                TestId = test.TestId,
                TestName = test.TestName,
                Description = test.Description,
                DurationInMinutes = test.DurationInMinutes,
                MaxScore = test.MaxScore,
                IsVisible = test.IsVisible,
                Order = test.Order,
                SubjectGradeId = test.SubjectGradeId
            }).ToList();
        }

    }
}
