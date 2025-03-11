using backend.Dtos;
using backend.Models;
using backend.Repositories;

namespace backend.Services
{
    public class StudentTestService
    {
        private readonly IStudentTestRepository _studentTestRepository;

        public StudentTestService(IStudentTestRepository studentTestRepository)
        {
            _studentTestRepository = studentTestRepository;
        }

        public async Task AddStudentTestAsync(StudentTestDto studentTestDto)
        {
            var newStudentTest = new StudentTest
            {
                StartTime = studentTestDto.StartTime,
                EndTime = studentTestDto.EndTime,
                UserId = studentTestDto.UserId,
                TestId = studentTestDto.TestId,
                Score = studentTestDto.Score   
            };

            await _studentTestRepository.AddStudentTest(newStudentTest);
        }
    }
}
