using backend.DataAccess.DAO;
using backend.Dtos.PracticeAndTest;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace backend.Services
{
    public class PracticeAttemptService
    {
        private readonly IPracticeRepository _practiceRepository;
        private readonly IStudentPerformanceRepository _studentPerformanceRepository;

        public PracticeAttemptService(IPracticeRepository practiceRepository, IStudentPerformanceRepository studentPerformanceRepository)
        {
            _practiceRepository = practiceRepository;
            _studentPerformanceRepository = studentPerformanceRepository;
        }

        public async Task AddPraticeAttempt(PracticeAttemptDto practiceAttemptDto)
        {
            var practiceAttempt = new PracticeAttempt
            {
                CorrectAnswers = practiceAttemptDto.CorrectAnswers,
                LevelId = practiceAttemptDto.Level,
                CreateAt = DateTime.UtcNow,
                TimePractice = practiceAttemptDto.TimePractice,
                UserId = practiceAttemptDto.UserId,
                LessonId = practiceAttemptDto.LessonId,
                SampleQuestion = practiceAttemptDto.SampleQuestion
            };
            await _practiceRepository.AddPracticeAttemp(practiceAttempt);
            await UpdateStudentPerformanceAsync(practiceAttemptDto.UserId, practiceAttemptDto.LessonId);
        }

        private async Task UpdateStudentPerformanceAsync(int userId, int lessonId)
        {
            var userAttempts = await _practiceRepository.GetUserAttemptsAsync(userId, lessonId);

            if (!userAttempts.Any()) return;

            double avgAccuracy = userAttempts.Average(a => (double)a.CorrectAnswers);
            Console.WriteLine("điểm trung bình là: " + avgAccuracy);
            double avgTimePerQuestion = userAttempts.Average(b => b.TimePractice);

            int newLevel = avgAccuracy < 5 ? 2 : (avgAccuracy < 8 ? 3 : 4);

            var studentPerformance = await _studentPerformanceRepository.GetOrCreateStudentPerformanceAsync(userId, lessonId);
            studentPerformance.avg_Accuracy = avgAccuracy;
            studentPerformance.LastAttempt = DateTime.UtcNow;
            studentPerformance.LevelId = newLevel;
            studentPerformance.avg_Time = avgTimePerQuestion;

            await _studentPerformanceRepository.UpdateStudentPerformanceAsync(studentPerformance);
        }
    }
}
