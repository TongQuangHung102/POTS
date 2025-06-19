using backend.DataAccess.DAO;
using backend.Dtos.PracticeQuestion;
using backend.Dtos.Questions;
using backend.Models;
using backend.Repositories;
using Microsoft.EntityFrameworkCore;

namespace backend.Services
{
    public class PracticeQuestionService
    {
        private readonly IPracticeQuestionRepository _practiceQuestionRepository;

        public PracticeQuestionService(IPracticeQuestionRepository practiceQuestionRepository)
        {
            _practiceQuestionRepository = practiceQuestionRepository;
        }

        public async Task SaveQuestionAndAnswersAsync(List<PracticeQuestionDto> questionList, int lessonId)
        {
            try
            {
                foreach (var question in questionList)
                {
                    if (question == null)
                        continue;

                    var practiceQuestion = new PracticeQuestion
                    {
                        QuestionText = question.questionText,
                        CorrectAnswer = question.correctAnswer,
                        LessonId = lessonId,
                        CreateAt = DateTime.UtcNow.AddHours(7)
                    };

                    var pQ = await _practiceQuestionRepository.CreateAsync(practiceQuestion);

                    var answerPracticeQuestion = question.answerQuestions.Select(n => new AnswerPracticeQuestion
                        {
                            AnswerText = n.answerText,
                            Number = n.number,
                            PracticeQuestionId = pQ.PracticeQuestionId
                        });

                    await _practiceQuestionRepository.AddAnswerQuestionsAsync(answerPracticeQuestion.ToList());
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException("Đã xảy ra lỗi đồng thời khi lưu dữ liệu.");
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("Đã xảy ra lỗi khi lưu dữ liệu vào cơ sở dữ liệu.");
            }
            catch (ArgumentNullException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Đã xảy ra lỗi không mong muốn khi lưu dữ liệu." + ex.Message);
            }
        }

        public async Task CleanupUnansweredPracticeQuestionsAsync()
        {
            var expiredTime = DateTime.UtcNow.AddHours(7).AddHours(-2);
            var oldQuestions = await _practiceQuestionRepository.GetExpiredUnansweredQuestionsAsync(expiredTime);

            if (!oldQuestions.Any()) return;

            _practiceQuestionRepository.DeletePracticeQuestions(oldQuestions);

            await _practiceQuestionRepository.SaveAsync();
        }
    }
}
