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
                        CreateAt = DateTime.Now
                    };

                    var pQ = await _practiceQuestionRepository.CreateAsync(practiceQuestion);

                    var answerPracticeQuestion = question.answerQuestions.Select(n => new AnswerPracticeQuestion
                        {
                            AnswerText = n.answerText,
                            Number = n.number,
                            QuestionId = pQ.QuestionId 
                        });

                    await _practiceQuestionRepository.AddAnswerQuestionsAsync(answerPracticeQuestion.ToList());
                }
            }
            catch (DbUpdateConcurrencyException ex)
            {
                throw new InvalidOperationException("A concurrency error occurred while saving data.");
            }
            catch (DbUpdateException ex)
            {
                throw new InvalidOperationException("An error occurred while saving the data to the database.");
            }
            catch (ArgumentNullException ex)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("An unexpected error occurred while saving the data." + ex.Message);
            }
        }
    }
}
