using backend.Models;

namespace backend.Repositories
{
    public interface IPracticeRepository
    {
        Task<List<PracticeAttempt>> GetUserAttemptsAsync(int userId, int lessonId);
         Task AddPracticeAttemp(PracticeAttempt practiceAttempt);
        Task<PracticeAttempt> GetLastAttempt(int userId, int lessonId);


    }
}
