using backend.Models;

namespace backend.Repositories
{
    public interface IStudentAnswerRepository
    {
        Task<StudentAnswer> CreateAsync(StudentAnswer studentAnswer);
        void DeleteStudentAnswers(List<StudentAnswer> studentAnswers);
        Task<List<StudentAnswer>> GetByPracticeIdAsync(int practiceId);
    }
}
