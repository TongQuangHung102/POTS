using backend.Models;

namespace backend.Repositories
{
    public interface IStudentAnswerRepository
    {
        Task<StudentAnswer> CreateAsync(StudentAnswer studentAnswer);
        Task<bool> DeleteAsync(int answerId);
        Task<List<StudentAnswer>> GetByPracticeIdAsync(int practiceId);
    }
}
