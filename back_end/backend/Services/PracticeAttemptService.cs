using backend.DataAccess.DAO;
using backend.Dtos;
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
                Time = practiceAttemptDto.Time,
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

            double avgAccuracy = userAttempts.Average(a => (double)a.CorrectAnswers / 10 );
            Console.WriteLine("điểm trung bình là: " + avgAccuracy);
            TimeSpan avgTimePerQuestion = TimeSpan.FromSeconds(userAttempts.Average(a => a.Time.TotalSeconds / 10));

            int newLevel = avgAccuracy < 5 ? 2 : (avgAccuracy < 8 ? 3 : 4);

            var studentPerformance = await _studentPerformanceRepository.GetOrCreateStudentPerformanceAsync(userId, lessonId);
            studentPerformance.avg_Accuracy = avgAccuracy;
            studentPerformance.avg_Time_Per_Question = avgTimePerQuestion;
            studentPerformance.LastAttempt = DateTime.UtcNow;
            studentPerformance.LevelId = newLevel;

            await _studentPerformanceRepository.UpdateStudentPerformanceAsync(studentPerformance);
        }
    }
}
