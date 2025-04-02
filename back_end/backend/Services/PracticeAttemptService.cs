using backend.DataAccess.DAO;
using backend.Dtos.PracticeAndTest;
using backend.Models;
using backend.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace backend.Services
{
    public class PracticeAttemptService
    {
        private readonly IPracticeRepository _practiceRepository;
        private readonly IStudentPerformanceRepository _studentPerformanceRepository;
        private readonly IStudentAnswerRepository _studentAnswerRepository;

        public PracticeAttemptService(IPracticeRepository practiceRepository, IStudentPerformanceRepository studentPerformanceRepository, IStudentAnswerRepository studentAnswerRepository)
        {
            _practiceRepository = practiceRepository;
            _studentPerformanceRepository = studentPerformanceRepository;
            _studentAnswerRepository = studentAnswerRepository;
        }

        public async Task AddPraticeAttempt(PracticeAttemptDto practiceAttemptDto)
        {
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                try
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

                    var practiceId = await _practiceRepository.AddPracticeAttempt(practiceAttempt);
                    if (practiceId <= 0)
                    {
                        throw new Exception("Lỗi khi thêm bài làm.");
                    }

                    await UpdateStudentPerformanceAsync(practiceAttemptDto.UserId, practiceAttemptDto.LessonId);

                    foreach (var answer in practiceAttemptDto.Answers)
                    {
                        var studentAnswer = new StudentAnswer
                        {
                            QuestionId = answer.QuestionId,
                            SelectedAnswer = answer.SelectedAnswer,
                            PracticeId = practiceId
                        };

                        await _studentAnswerRepository.CreateAsync(studentAnswer);
                    }

                    transaction.Complete();
                }
                catch
                {
                    throw;
                }
            }
        }


        private async Task UpdateStudentPerformanceAsync(int userId, int lessonId)
        {
            var userAttempts = await _practiceRepository.GetUserAttemptsAsync(userId, lessonId);

            if (!userAttempts.Any()) return;

            double avgAccuracy = userAttempts.Average(a => (double)a.CorrectAnswers);
            double avgTimePerQuestion = userAttempts.Average(b => b.TimePractice);

            int newLevel = avgAccuracy < 5 ? 2 : (avgAccuracy < 8 ? 3 : 4);

            var studentPerformance = await _studentPerformanceRepository.GetOrCreateStudentPerformanceAsync(userId, lessonId);
            studentPerformance.avg_Accuracy = avgAccuracy;
            studentPerformance.LastAttempt = DateTime.UtcNow;
            studentPerformance.LevelId = newLevel;
            studentPerformance.avg_Time = avgTimePerQuestion;

            await _studentPerformanceRepository.UpdateStudentPerformanceAsync(studentPerformance);
        }

        public async Task<(List<PracticeAttempt> Attempts, int TotalCount)> GetHistoryByLessonAndUserAsync(
        int lessonId, int userId, int pageNumber, int pageSize)
        {
            try
            {
                return await _practiceRepository.GetPracticeAttemptsByLessonAndUserAsync(lessonId, userId, pageNumber, pageSize);
            }
            catch (Exception ex)
            {
                return (new List<PracticeAttempt>(), 0);
            }
        }

        public async Task<PracticeAttempt> GetPracticeAttemptDetailAsync(int practiceId)
        {
            try
            {
                var attempt = await _practiceRepository.GetPracticeAttemptDetailAsync(practiceId);
                if (attempt == null)
                {
                    throw new InvalidOperationException($"Không tìm thấy bài làm với ID: {practiceId}");
                }
                return attempt;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
